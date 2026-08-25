# InfectionHUD

This mod displays the infection % caused by the Cadaver to the player.
It is a client side mod. 

## Development Tips
- Use [ScriptEngine](https://github.com/BepInEx/BepInEx.Debug/releases) for hot-reload support, providing great development experience. NO MORE restarting and finding a Cadaver Growth for 10 minutes every code change!
- Use the Rider publish configuration to build and copy the mod to the BepInEx plugins folder automatically.

### IMPORTANT NOTE:
This is not documented anywhere, I had to waste a Saturday for this, but set `HideManagerGameObject = true` in `BepInEx/config/BepInEx.cfg`
The default value `false` is incompatible with ScriptEngine and causes BepInEx plugin manager object to be destroyed early.
Thank Lethal Company for this behavior!

For production, be sure to test it works with it false !!
