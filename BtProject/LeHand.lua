LF = 0 --Little Finger
RF = 1 --Ring Finger
MF = 2 --Middle Finger
IF = 3 --Index Finger
Th = 4 --Thumb
AX = 5 --Axis X
AY = 6 --Axis Y
AZ = 7 --Axis Z
RY = 8 --Rotation Y
Prot9 = 9 -- undefined, for later use
Prot10 =10
Prot11 =11
Prot12 =12
Prot13 =13
Prot14 =14
Prot15 =15
Prot16 =16

Mode_Press = 0
Mode_Absolute = 1

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

LButton = 2 + 4
RButton = 8 + 16

Update = function()
    
end

function Start()
  while true do
    val = RecvVal()
    LF = val[0] 
    RF = val[1]
    MF = val[2]
    IF = val[3]
    Th = val[4]
    AX = val[5]
    AY = val[6]
    AZ = val[7]
    RY = val[8]
    go = Update()
    if go == false then break end
  end
  
  Exit()
end