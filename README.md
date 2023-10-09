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


## Setup
Required environment variables:
```
# Speech-to-text service
KEYVOX_AZ_S2T_API_KEY
KEYVOX_AZ_S2T_REGION

# OpenAI (Azure)
KEYVOX_AZ_AI_URL
KEYVOX_AZ_AI_API_KEY
```
