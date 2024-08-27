#! /bin/bash



FROM_UNITY_SCRIPTS=~/"Make/Unity-Projects/creative name 1.0/Assets/Scripts"
TO_STUFF=~/"Make/creative name 1.0 stuff"


# echo pwd
# pwd
# echo echo
# echo  "$TO_STUFF"
# echo cd
# cd  "$TO_STUFF"
# echo pwd
# pwd



cp -uv  "$FROM_UNITY_SCRIPTS"/"CameraController.cs"                 "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"GameController.cs"                   "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"ItemDashCollector.cs"                "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"ItemKeyCollector.cs"                 "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"LevelChanger.cs"                     "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"LockedCage.cs"                       "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"PlayerController.cs"                 "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"SceneOnLoadData.cs"                  "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"TerrainHeightResolutionFixer.cs"     "$TO_STUFF"/"Scripts bckp"
cp -uv  "$FROM_UNITY_SCRIPTS"/"UIController.cs"                     "$TO_STUFF"/"Scripts bckp"
