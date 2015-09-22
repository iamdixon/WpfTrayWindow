@ECHO off



IF NOT EXIST temp MD temp

SET "sizes=16 24 32 48 57 64 72 96 120 128 144 152 195 228 256 512"

FOR /r %%i in (*.svg) DO (

	FOR %%s IN (%sizes%) DO (
		inkscape -z -e temp/%%~ni-%%s.png -w %%s -h %%s %%i
	)
	convert temp/%%~ni-16.png temp/%%~ni-24.png temp/%%~ni-32.png temp/%%~ni-48.png temp/%%~ni-64.png %%~ni.ico
	RMDIR temp /S /Q
)
