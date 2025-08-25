docker run --rm -it -p 5000:5000 --memory 8g --cpus 1 \
mcr.microsoft.com/azure-cognitive-services/textanalytics/sentiment:latest \
Eula=accept \
Billing=https://tempaiservicesdclanguage.cognitiveservices.azure.com/ \
ApiKey=<apikey>