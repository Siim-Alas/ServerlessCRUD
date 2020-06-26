Blazor UI deployment

https://anthonychu.ca/post/blazor-azure-storage-static-websites/

copy BlazorUI/bin/Release/netstandard2.1/publish/wwwroot to $web

az storage blob upload-batch --connection-string <connection_string> -s . -d $web