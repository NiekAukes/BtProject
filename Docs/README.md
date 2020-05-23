# Headline

> An awesome project.
```Lua
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
    if X % 3000 == 0 then
        --Kpress("a", 0)
        Mpress(Button_MLD + Button_MLU)
        Sys("cls")
        return false
    end
    return true
end

Start()

--returning false will end the current session
```