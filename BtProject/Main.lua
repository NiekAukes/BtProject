LeHand = require "LeHand"
--Functions:
----Kpress(Character, Mode) -> registers Keypress
----Mpress(Button, doRelease) -> registers Mousepress
----Mmove(xDir,yDir)
----delay(time in ms)

--begin, will be executed on startup

Button_MLD = 0x0002 -- left button down
Button_MLU = 0x0004 --left button up
Button_MRD = 0x0008 --right button down
Button_MRU = 0x0010 --right button up
Button_MMD = 0x0020 --middle button down
Button_MMU = 0x0040 --middle button up
Button_MXD = 0x0080 --x button down
Button_MXU = 0x0100 --x button up
Button_MFW = 0x0800 --wheel button rolled
Button_MFHW = 0x01000 --hwheel button rolled

print("Hello LeHand")

X = false
Update = function ()
		
	--[[print("1: " .. val[1])
	print("2: " .. val[2])
	print("3: " .. val[3])
	print("4: " .. val[4])]]--
	
	if(val[1] > 800) then
	Mpress(Button_MLD + Button_MLU, doRelease)
	end
	
	if(val[2] > 800) then
	Mpress(Button_MRD + Button_MRU, doRelease)
	end
	
	if(val[3] > 600) then
	MMove(0, -((val[3] - 600) / 300.0), 0)
	end
	if(val[3] < 400) then
	MMove(0, (400 - val[3]) / 300.0, 0)
	end
	
	--os.execute("timeout " .. tonumber(1))
	
    --will be executed every frame
    --[[if (X) then
        if (val[1] < 0.5) then
            print("HIGH ".. val[1])
            X = false
        end
    else
        if (val[1] > 0.5) then
            print("LOW ".. val[1])
            X = true
        end]]--

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
        end 
    end]]--

    return true	--will continue program
end

Start()


--returning false will end the current session
