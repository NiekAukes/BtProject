LeHand = require "LeHand"
--Functions:
----Kpress(Character, Mode) -> registers Keypress
----Mpress(Button, doRelease) -> registers Mousepress


--begin, will be executed on startup
print("Hello LeHand")

X = false
Update = function ()
    --will be executed every frame
    if (X) then
        if (val[1] < 0.5) then
            print("HIGH ".. val[1])
            X = false
        end
    else
        if (val[1] > 0.5) then
            print("LOW ".. val[1])
            X = true
        end

    --X = X + 1
    
    --[[
    if X % 3000 == 0 then
        
        x = val[2]
        print(x)
        --print(string.rep(" ", x) .. "|")
        if (val[2] < 0.5) then
            Mpress(Button_MLD)
        else
            Mpress(Button_MLU)
        end ]]--
    end

    return true	--will continue program
end

Start()


--returning false will end the current session
