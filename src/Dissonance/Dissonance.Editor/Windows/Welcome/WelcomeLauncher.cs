using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissonance.Editor.Windows.Welcome
{
    [InitializeOnLoad]
    public class WelcomeLauncher
    {
        private static readonly string StatePath = Path.Combine(DissonanceRootPath.BaseResourcePath, ".WelcomeState.json");

        // Add a menu item to launch the window
        [MenuItem("Window/Dissonance/Integrations"), UsedImplicitly]
        private static void ShowWindow()
        {
            //Clear installer state
            File.Delete(StatePath);

            //Run startup to show the window
            Startup();
        }

        internal static void Startup()
        {
            var state = GetWelcomeState();

            if (!state.ShownForVersion.Equals(DissonanceComms.Version.ToString()))
            {
                SetWelcomeState(new WelcomeState(DissonanceComms.Version.ToString()));
                WelcomeWindow.ShowWindow(state);
            }
        }

        [NotNull] private static WelcomeState GetWelcomeState()
        {
            if (!File.Exists(StatePath))
            {
                // State path does not exist at all so create the default
                var state = new WelcomeState("");
                SetWelcomeState(state);
                return state;
            }
            else
            {
                //Read the state from the file
                using (var reader = File.OpenText(StatePath))
                    return JsonUtility.FromJson<WelcomeState>(reader.ReadToEnd());
            }
        }

        private static void SetWelcomeState([CanBeNull]WelcomeState state)
        {
            if (state == null)
            {
                //Clear installer state
                File.Delete(StatePath);
            }
            else
            {
                using (var writer = File.CreateText(StatePath))
                    writer.Write(JsonUtility.ToJson(state));
            }
        }
    }
}