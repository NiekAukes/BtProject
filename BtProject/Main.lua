LeHand = require "LeHand"
--Functions:
----Kpress(Character, Mode) -> registers Keypress
----Mpress(Button, doRelease) -> registers Mousepress
----Exit() -> exits the program


--begin, will be executed on startup
print("Hello LeHand")

X = 1
while true do
    --will be executed every frame
    print(X .. "times executed")
    X = X + 1
    if X > 5000 then
        --Kpress("a", 0)
        Mpress(Button_MLD + Button_MLU)
        break
    end
end

--breaking the loop will end the current session
