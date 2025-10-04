using StardewValley;

namespace DedicatedServer.Helpers
{
    /// <summary>
    /// The underlying logic of the properties was copied from the following decompiled method: <see cref="SaveGame.getSaveEnumerator()"/>
    /// </summary>
    public static class SaveGameHelper
    {
        /// <summary>
        /// For example: <c> Stardew </c>
        /// </summary>
        public static string FriendlyName { get => SaveGame.FilterFileName(Game1.GetSaveGameName()); }

        /// <summary>
        /// For example: <c> Stardew_418064419 </c>
        /// </summary>
        public static string Text { get => FriendlyName + "_" + Game1.uniqueIDForThisGame; }

        /// <summary>
        /// For example: <c> %APPDATA%/StardewValley/Saves/Stardew_418064419/ </c>
        /// </summary>
        public static string Path { get => System.IO.Path.Combine(Program.GetSavesFolder(), Text + System.IO.Path.DirectorySeparatorChar); }

        /// <summary>
        /// For example: <c> %APPDATA%/StardewValley/Saves/Stardew_418064419/SaveGameInfo </c>
        /// </summary>
        public static string FinalFarmerPath { get => System.IO.Path.Combine(Path, "SaveGameInfo"); }

        /// <summary>
        /// For example: <c> %APPDATA%/StardewValley/Saves/Stardew_418064419/Stardew_418064419 </c>
        /// </summary>
        public static string FinalDataPath { get => System.IO.Path.Combine(Path, Text); }

        /// <summary>
        /// For example: <c> %APPDATA%/StardewValley/Saves/Stardew_418064419/SaveGameInfo_STARDEWVALLEYSAVETMP </c>
        /// </summary>
        public static string TempFarmerPath { get => FinalFarmerPath + SaveGame.TempNameSuffix; }

        /// <summary>
        /// For example: <c> %APPDATA%/StardewValley/Saves/Stardew_418064419/Stardew_418064419_STARDEWVALLEYSAVETMP </c>
        /// </summary>
        public static string TempDataPath { get => FinalDataPath + SaveGame.TempNameSuffix; }

    }
}
