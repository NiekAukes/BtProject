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
short reads[] = {duim}; //DEBUG VERSIE

//reads wordt ook gebruikt voor header informatie

const short data_length = 4; //length of double
const short footer =      0xFFFF; //'exit' code of information
short data_arr[data_length];
double read_inf;

void setup() {
  Serial.begin(9600);
  pinMode(duim,INPUT);
}



void loop() {
  //Read information
  for(int i = 0; i < sizeof(reads)/sizeof(int);i++){
    
    read_inf = (analogRead(pot)/1023.0);
    
    //Cut information to shorts
    for(int j = 0; j < data_length; j++){
      data_arr[j] = *(((short*)&read_inf) + j);
    }
    
    ////Header, information, footer print to serial
    Serial.write((char*)&data_length, 2);//header
    Serial.write((char*)&reads[i], 2);//data length
    Serial.write((char*)data_arr,sizeof(data_arr)); //speciale functie Serial.write, arrays zijn eigenlijk char pointers,
    Serial.write(0xFF); Serial.write(0xFF);         //hier mee verstuur je dus gewoon een array

    //Serial.println(int(float(analogRead(A0)) * (1024.0/1023.0)));
  }
}
