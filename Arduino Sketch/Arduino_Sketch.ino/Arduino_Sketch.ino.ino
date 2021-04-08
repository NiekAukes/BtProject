/* Header       0x0001 (wat wordt er gemeten? hand, gyroscoop, acceleratie etc.)
   Data_length  0x000i (split de double in 4 shorts)
   Data     (ix)0x????
   Footer       0xFFFF (om de data stream af te sluiten zodat de ontvanger weet dat er niet meer komt) 
*/
//double = 8bytes 64bit |||| short = 2bytes 16bit

#include <SoftwareSerial.h>
#define pot1          A0
#define pot2          A1
#define rx            0
#define tx            1
SoftwareSerial BTSerial(rx, tx);

//different fingers
#define duim          2
#define wijs_ving     3
#define mid_ving      4
#define ring_ving     5
#define pink          6 

//gyroscope
#define gyr_x         7
#define gyr_y         8
#define gyr_z         9

//accelerometer
#define acc_x         10
#define acc_y         11
#define acc_z         12

short reads[] = {pot1,pot2,mid_ving,ring_ving,pink,gyr_x,gyr_y,gyr_z,acc_x,acc_y,acc_z};
short id[]    = {1,2,3,4,5,6,7,8,9,10,11};

//short reads[] = {pot1,pot2}; //DEBUG VERSIE

//reads wordt ook gebruikt voor header informatie

const short data_length = 4; //length of double
const short footer =      0xFFFF; //'exit' code of information
short data_arr[data_length];

bool active = false;
float read_inf;

void setup() {
  Serial.begin(9600); //was 9600
  pinMode(pot1,INPUT);
  pinMode(pot2,INPUT);
  
  pinMode(5, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.setTimeout(20);
}



void loop() {
  if (Serial.available()) {
    Serial.println("GOT\n");
    String str = Serial.readString();
    Serial.println("TRIM\n");
    str.trim();
    
    if (str == "OK") {
      active = true;
      digitalWrite(5, HIGH);
      Serial.write("OK");
    }
    if (str == "STOP") {
      active = false;
      
      digitalWrite(5, LOW);
    }
    if (str == "TEST") {
      active = false;
      //delay(1000);
      digitalWrite(5, HIGH);
      Serial.write("OK");
      delay(300);
      digitalWrite(5, LOW);
    }
  }
  
  //Read information
  if (active) 
  {
    for(int i = 0; i < sizeof(reads)/sizeof(int);i++){
      
      read_inf = (analogRead(reads[i])/1023.0);
      //digitalWrite(LED_BUILTIN, HIGH);
      if (read_inf < 0.5)
        digitalWrite(LED_BUILTIN, HIGH);
      else 
        digitalWrite(LED_BUILTIN, LOW);
      
      ////Header, information, footer print to serial
      Serial.write((char*)&id[i], 2);//data length
      Serial.write((char*)&data_length, 2);//header
      //information
      Serial.write((char*)&read_inf,4); //size 8 = double (8 bytes)
      //footer
      Serial.write(0xFF); 
      Serial.write(0xFF);
      //delay(50);
      //Serial.println(int(float(analogRead(A0)) * (1024.0/1023.0)));
    }
  }
  
}
