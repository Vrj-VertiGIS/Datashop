del /F /Q .\.configuration\%CONFIGURATION_DIR%\*
rd .\.configuration\%CONFIGURATION_DIR%\
mkdir  .\.configuration\%CONFIGURATION_DIR%
@echo This content is meaningless > .\.configuration\%CONFIGURATION_DIR%\%CONFIGURATION_NAME%
@echo[
@echo ======== Switched to configuration %CONFIGURATION_NAME%  ========
@echo[


