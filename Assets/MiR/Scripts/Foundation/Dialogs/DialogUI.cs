using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    public sealed class DialogUI : AbstractService<IDialogUI>, IDialogUI
    {
        public SceneState State;
        public Transform MessagesContainer;
        public Image Background;
        public Image LeftPortrait;
        public Image RightPortrait;
        public LocalizedString EndDialogMessage;

        float leftPortraitX;
        float rightPortraitX;
        float backgroundOpacity;

        [Inject] DialogRow.Factory dialogRowFactory = default;
        [Inject] ISceneStateManager manager = default;
        [Inject] IQuestManager questManager = default;
        [Inject] ILocalizationManager localizationManager = default;

        List<DialogRow> rows = new List<DialogRow>();

        public override void Start()
        {
            base.Start();
            leftPortraitX = LeftPortrait.transform.localPosition.x;
            rightPortraitX = RightPortrait.transform.localPosition.x;
            backgroundOpacity = Background.color.a;
        }

        public void DisplayDialogs(IPlayer player, Sprite portrait, List<Dialog> dialogs)
        {
            LeftPortrait.sprite = (player != null ? player.Portrait : null);
            RightPortrait.sprite = portrait;

            StartCoroutine(DialogCoroutine(dialogs));
        }

        IEnumerator DialogCoroutine(List<Dialog> dialogs)
        {
            Clear();
            manager.Push(State);

            var dialogTask = ShowDialogs(dialogs);
            while (!dialogTask.IsCompleted)
                yield return null;

            manager.Pop(State);

            dialogTask.Wait(); // убеждаемся, что возможные исключения обработаны
        }

        async Task ShowDialogs(List<Dialog> dialogs)
        {
            const float BackgroundAppearTime = 0.2f;
            const float BackgroundDisappearTime = 0.2f;
            const float PortraitsAppearTime = 1.0f;
            const float PortraitsDisappearTime = 1.0f;

            const float PortraitOffset = 300.0f;

            Background.DOFade(backgroundOpacity, BackgroundAppearTime).From(0.0f);
            LeftPortrait.transform.DOLocalMoveX(leftPortraitX, PortraitsAppearTime).From(leftPortraitX - PortraitOffset);
            await RightPortrait.transform.DOLocalMoveX(rightPortraitX, PortraitsAppearTime).From(rightPortraitX + PortraitOffset).AsyncWaitForCompletion();

            List<DialogNode> rootNodes = new List<DialogNode>();
            do {
                rootNodes.Clear();
                foreach (var dialog in dialogs) {
                    foreach (var node in dialog.RootNodes) {
                        if (node.CanShow(questManager))
                            rootNodes.Add(node);
                    }
                }
            } while (await ShowOptions(rootNodes, true));

            Clear();

            Background.DOFade(0.0f, BackgroundDisappearTime);
            LeftPortrait.transform.DOLocalMoveX(leftPortraitX - PortraitOffset, PortraitsDisappearTime);
            await RightPortrait.transform.DOLocalMoveX(rightPortraitX + PortraitOffset, PortraitsDisappearTime).AsyncWaitForCompletion();
        }

        async Task<bool> ShowOptions(List<DialogNode> dialogs, bool allowEndDialog)
        {
            if (dialogs.Count == 0)
                return false;

            do {
                DialogNode selectedNode = null;
                if (dialogs.Count == 1 && !allowEndDialog) {
                    var row = CreateRow(dialogs[0].IsPlayer);
                    row.Text.text = localizationManager.GetString(dialogs[0].Text);
                    row.Text.gameObject.SetActive(true);
                    row.BalloonImage.GetComponent<Image>().color = Color.red; // FIXME
                    selectedNode = dialogs[0];
                } else {
                    var row = CreateRow(true);
                    row.Text.gameObject.SetActive(false);

                    foreach (var dialog in dialogs) {
                        if (dialog.CanShow(questManager))
                            row.AddButton(dialog.Text.LocalizationID, localizationManager.GetString(dialog.Text), dialog);
                    }
                    if (allowEndDialog)
                        row.AddButton(EndDialogMessage.LocalizationID, localizationManager.GetString(EndDialogMessage), null);

                    do {
                        await Task.Yield();
                    } while (!row.TryGetSelected(out selectedNode));

                    row.Text.text = localizationManager.GetString(selectedNode != null ? selectedNode.Text : EndDialogMessage);
                    row.Text.gameObject.SetActive(true);
                    row.BalloonImage.GetComponent<Image>().color = Color.red; // FIXME
                    row.RemoveButtons();

                    if (selectedNode == null)
                        return false;
                }

                selectedNode.SetHasBeenUsed();

                switch (selectedNode.NodeAction) {
                    case DialogNode.Action.Default:
                        break;

                    case DialogNode.Action.StartQuest:
                        questManager.StartQuest(selectedNode.ActionQuest);
                        break;
                }

                allowEndDialog = false;
                dialogs = selectedNode.Next;
            } while (dialogs != null && dialogs.Count > 0);

            return true;
        }

        DialogRow CreateRow(bool isPlayer)
        {
            var row = dialogRowFactory.Create();
            row.transform.SetParent(MessagesContainer, false);

            var scale = row.BalloonImage.localScale;
            scale.x = (isPlayer ? 1.0f : -1.0f);
            row.BalloonImage.localScale = scale;

            row.BalloonImage.GetComponent<Image>().color = Color.white; // FIXME

            rows.Add(row);

            // FIXME: этот хак обесечивает корректное обновление UI при первом отображении диалога
            var vlg = MessagesContainer.GetComponent<VerticalLayoutGroup>();
            vlg.enabled = false;
            DOVirtual.DelayedCall(0.001f, () => { vlg.enabled = true; }, ignoreTimeScale: true);

            return row;
        }

        void Clear()
        {
            foreach (var row in rows)
                row.Pool.Despawn(row);
            rows.Clear();
        }
    }
}
