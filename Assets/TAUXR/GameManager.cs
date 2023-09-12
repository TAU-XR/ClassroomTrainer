
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : TXRSingleton<GameManager>
{
    [SerializeField] private bool _shouldProjectUseCalibration;     // true if project should be calibrated into a physical space.
    [SerializeField] private bool _shouldCalibrateOnEditor;
    private TXRSceneManager _sceneManager;
    private EnvironmentCalibrator _calibrator;

    private void Start()
    {
        _calibrator = EnvironmentCalibrator.Instance;
        _sceneManager = TXRSceneManager.Instance;

        StartTAUXRExperience().Forget();
    }

    private async UniTask StartTAUXRExperience()
    {
        bool shouldCalibrateOnBuild = !Application.isEditor && _shouldProjectUseCalibration;
        bool shouldCalibrateOnEditor = _shouldCalibrateOnEditor && Application.isEditor && _shouldProjectUseCalibration;

        if (shouldCalibrateOnBuild || shouldCalibrateOnEditor)
        {
            // trigger calibration on BaseScene
            await _calibrator.CalibrateRoom();
        }

        // after environment was calibrated- load first scene
        _sceneManager.Init(_shouldProjectUseCalibration);
    }

}
