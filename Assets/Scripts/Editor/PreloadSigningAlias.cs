using UnityEditor;

[InitializeOnLoad]
public class PreloadSigningAlias
{
    static PreloadSigningAlias()
    {
        PlayerSettings.Android.keystoreName = "Deploy/zominthos.keystore";
        PlayerSettings.Android.keystorePass = "!diadrasis$";
        PlayerSettings.Android.keyaliasName = "zominthos";
        PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass;

    }
}
