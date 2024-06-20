TASKKILL /IM "ValorantPorting.exe" /F
TIMEOUT /t 1 /nobreak > NUL
MOVE "ValorantPorting.temp.exe" "ValorantPorting.exe"
START "" /B "ValorantPorting.exe"