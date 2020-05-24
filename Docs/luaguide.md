# How to use Lua in Lehand

Here, we will break down this code:
```lua
LeHand = require "LeHand"
--Functions:
----Kpress(Character, Mode) -> registers Keypress
----Mpress(Button, doRelease) -> registers Mousepress


--begin, will be executed on startup
print("Hello LeHand")

X = 1
Update = function ()
    --will be executed every frame
    print(X .. "times executed")
    X = X + 1
    if X > 3000 then
        Kpress("a", 0)
        Mpress(Button_MLD + Button_MLU)
        Sys("cls")
        return false
    end
    return true
end

Start()

--returning false will end the current session
```
This code will count until X is 3000, then presses the 'a' button and clicks the leftmouse.<br>
It will then clear the console via a syscall and close the program by returning false
<br><br>
# Breakdown

```lua
LeHand = require "LeHand"
```
This line will include the LeHand library, this is required in every LeHand program.<br><br>

```lua
print("Hello LeHand")

X = 1
```

These are for initialization. Everything put here will be run on startup.<br><br>

```lua
Update = function ()
    --will be executed every frame
	...
end
```
This line will override the update function. everything put here will be executed every frame.<br>
make sure to use tabs to indicate you are in the 