# KeyVox

![image](https://github.com/Navusas/KeyVox/assets/32360417/20ba9d92-d31f-465e-ac2b-e32fec6ee174)

**Just `CTRL + SHIFT + A` away from AI!**

Here is brief introduction on how this app works:
```
Select Text --> Hit our app (CTLR + SHIFT + A) --> Speech-to-text --> ChatGPT --> Result
```

Some details:
- We use Azure Speech-to-text.
- We use Azure OpenAI client (with ChatGPT-4 access, for now).


## Start
**App works on Windows only, as of now**

```
# git clone repo
...

# Go inside cloned repo
cd KeyVox/

# Set environment variables
# "KEYVOX_AZ_AI_API_KEY"
# "KEYVOX_AZ_SPEECH_RECOGNITION_API_KEY"
# "KEYVOX_AZ_SPEECH_RECOGNITION_REGION"

# Run ./build.ps1
./build.ps1

# Run ./start.ps1
./start.ps1

```