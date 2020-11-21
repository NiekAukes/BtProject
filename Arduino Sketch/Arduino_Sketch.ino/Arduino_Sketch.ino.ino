/* Header       0x0001 (wat wordt er gemeten? hand, gyroscoop, acceleratie etc.)
   Data_length  0x000i (split de double in 4 shorts)
   Data     (ix)0x????
   Footer       0xFFFF (om de data stream af te sluiten zodat de ontvanger weet dat er niet meer komt) 
*/
//double = 8bytes 64bit |||| short = 2bytes 16bit

#include <SoftwareSerial.h>
#define pot           A0
#define rx            0
#define tx            1
SoftwareSerial BTSerial(rx, tx);

//different fingers
#define duim          A0
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

//short reads[] = {duim,wijs_ving,mid_ving,ring_ving,pink,gyr_x,gyr_y,gyr_z,acc_x,acc_y,acc_z};
short reads[] = {0}; //DEBUG VERSIE

//reads wordt ook gebruikt voor header informatie

const short data_length = 4; //length of double
const short footer =      0xFFFF; //'exit' code of information
short data_arr[data_length];
float read_inf;

void setup() {
  Serial.begin(9600);
  pinMode(duim,INPUT);
  pinMode(LED_BUILTIN, OUTPUT);
}



void loop() {
  //Read information
  for(int i = 0; i < sizeof(reads)/sizeof(int);i++){
    
    read_inf = (analogRead(pot)/1023.0);
    //digitalWrite(LED_BUILTIN, HIGH);
    if (read_inf < 0.5)
      digitalWrite(LED_BUILTIN, HIGH);
    else 
      digitalWrite(LED_BUILTIN, LOW);
    
    ////Header, information, footer print to serial
    Serial.write((char*)&reads[i], 2);//data length
    Serial.write((char*)&data_length, 2);//header
    //information
    /*char buf[24]; // "-2147483648\0"
    Serial.print(read_inf); Serial.print(" : ");
    for(int j = 0; j < 8; j++) {
      char* ch = itoa((int)(*(((char*)&read_inf) + j)), buf, 10);
      Serial.print(ch); Serial.print(" ");
    }
    Serial.print("\n");
    */
    Serial.write((char*)&read_inf,4); //size 8 = double (8 bytes)
    //footer
    Serial.write(0xFF); 
    Serial.write(0xFF);
    //delay(50);
    //Serial.println(int(float(analogRead(A0)) * (1024.0/1023.0)));
  }
}
