set "ROOTDIR=%cd%"
set "FiVES_DIR=%ROOTDIR%\FiVES"
set "FiVES_BVH_EXT_DIR=%ROOTDIR%\FiVES_BVH_Ext"

IF NOT EXIST %FiVES_DIR% Exit /b
IF NOT EXIST %FiVES_BVH_EXT_DIR% Exit /b

echo %FiVES_DIR%
echo %FiVES_BVH_EXT_DIR%

mkdir %FiVES_DIR%\Plugins\BVHAnimation
xcopy %FiVES_BVH_EXT_DIR%\Plugins\BVHAnimation %FiVES_DIR%\Plugins\BVHAnimation /s /y

mkdir %FiVES_DIR%\WebClientTrw
xcopy %FiVES_BVH_EXT_DIR%\WebClientTrw %FiVES_DIR%\WebClientTrw /s /y

copy %FiVES_BVH_EXT_DIR%\Plugins\Avatar\AvatarPluginInitializer.cs %FiVES_DIR%\Plugins\Avatar\  /y
copy %FiVES_BVH_EXT_DIR%\FIVES.sln %FiVES_DIR%\  /y