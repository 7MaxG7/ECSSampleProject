using Cysharp.Threading.Tasks;
using Utils;

namespace UI.Permanent
{
    public class CurtainService
    {
        private CurtainUIModel _curtainUIModel;

        public void Init(CurtainUIModel curtainUIModel)
        {
            _curtainUIModel = curtainUIModel;
        }

        public async UniTask Show()
            => await _curtainUIModel.IsActive.UpdateAsync(true);

        public async UniTask Hide()
            => await _curtainUIModel.IsActive.UpdateAsync(false);

        public void ShowInstantly()
        {
            _curtainUIModel.IsInstantAnimation = true;
            _curtainUIModel.IsActive.Update(true);
            _curtainUIModel.IsInstantAnimation = false;
        }
    }
}