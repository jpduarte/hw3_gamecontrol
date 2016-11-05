/* Graph I2C Accelerometer On RedBear Duo over Serial Port
 * Adafruit Part 2809 LIS3DH - http://adafru.it/2809
 * This example shows how to program I2C manually
 * I2C Pins SDA1==D0, SCL1 == D1
 * Default address: 0x18
 */
 
// do not use the cloud functions - assume programming through Arduino IDE
#if defined(ARDUINO) 
SYSTEM_MODE(MANUAL); 
#endif
// Basic demo for accelerometer readings from Adafruit LIS3DH

#include "Adafruit_LIS3DH.h"
#include "Adafruit_Sensor.h"

#define PI 3.1415
#define button D8

#define vcc 3.3;

//values from mat matrix
int mat_values[96];

//analog input
int analog_read = A1;

//rows control
int row0 = D2;
int row1 = D3;
int row2 = D4;
//byte rwo[] = {D2,D3,D4};

//column control
int column0 = D9;
int column1 = D10;
int column2 = D11;
int column3 = D12;

typedef struct ACCEL_DATA {
  float xG, yG, zG;
  float pitch, roll;
};

// Accelerometer 
ACCEL_DATA accel_data;
// I2C
Adafruit_LIS3DH lis = Adafruit_LIS3DH();

void setup(void) {
  
  Serial.begin(9600);
  pinMode(button,INPUT_PULLUP);
  pinMode(row0, OUTPUT);
  pinMode(row1, OUTPUT);  
  pinMode(row2, OUTPUT);  

  pinMode(column0, OUTPUT);  
  pinMode(column1, OUTPUT);  
  pinMode(column2, OUTPUT);  
  pinMode(column3, OUTPUT);  

  pinMode(analog_read, INPUT);   
  
  LIS3DH_init();
}

void loop() {
  LIS3DH_getData();
  get_mat_values();
  LIS3DH_sendData();
  MAT_sendData();
  Serial.printf("\n");
  delay(33); //30Hz update rate
}
void MAT_sendData(void) {
  for (int index = 0;index<96;index++) {
     Serial.printf("%i,",mat_values[index]);  
  }  
}

float read_mat(void){
  int analog_value;
  float analog_value_float;
  
  analog_value = analogRead(analog_read);
  analog_value_float = analog_value * vcc ; 
  analog_value_float = analog_value_float /  4095.0;
  return analog_value_float;
}

void get_mat_values(void){

  int index=0;
  //loop row first
  for (int rowcount = 0;rowcount<8;rowcount++) {
     set_row(rowcount);
     //then loop columns
     for (int columncount = 0;columncount<12;columncount++) {
      set_column(columncount);
      //save value at each point of mat, just read and save integer value
      mat_values[index]=analogRead(analog_read); //read_mat();
      //Serial.printf("rowcount: %i, columncount: %i, index: %i, value: %i \n",rowcount,columncount,index,mat_values[index]);
      index=index+1;
     }
  }
  
}

void set_column(int column) {
  switch (column) {
    case 0:
      digitalWrite(column0,LOW);
      digitalWrite(column1,LOW);
      digitalWrite(column2,LOW);
      break;
    case 1:
      digitalWrite(column0,HIGH);
      digitalWrite(column1,LOW);
      digitalWrite(column2,LOW);
      digitalWrite(column2,LOW);
      break;
    case 2:
      digitalWrite(column0,LOW);
      digitalWrite(column1,HIGH);
      digitalWrite(column2,LOW);
      digitalWrite(column2,LOW);
      break;
    case 3:
      digitalWrite(column0,HIGH);
      digitalWrite(column1,HIGH);
      digitalWrite(column2,LOW);
      digitalWrite(column2,LOW);
      break;
    case 4:
      digitalWrite(column0,LOW);
      digitalWrite(column1,LOW);
      digitalWrite(column2,HIGH);
      digitalWrite(column2,LOW);
      break;
    case 5:
      digitalWrite(column0,HIGH);
      digitalWrite(column1,LOW);
      digitalWrite(column2,HIGH);
      digitalWrite(column2,LOW);
      break;
    case 6:
      digitalWrite(column0,LOW);
      digitalWrite(column1,HIGH);
      digitalWrite(column2,HIGH);
      digitalWrite(column2,LOW);
      break;
    case 7:
      digitalWrite(column0,HIGH);
      digitalWrite(column1,HIGH);
      digitalWrite(column2,HIGH);
      digitalWrite(column2,LOW);
      break;      
    case 8:
      digitalWrite(column0,LOW);
      digitalWrite(column1,LOW);
      digitalWrite(column2,LOW);
      digitalWrite(column2,HIGH);
      break;
    case 9:
      digitalWrite(column0,HIGH);
      digitalWrite(column1,LOW);
      digitalWrite(column2,LOW);
      digitalWrite(column2,HIGH);
      break;
    case 10:
      digitalWrite(column0,LOW);
      digitalWrite(column1,HIGH);
      digitalWrite(column2,LOW);
      digitalWrite(column2,HIGH);
      break;
    case 11:
      digitalWrite(column0,HIGH);
      digitalWrite(column1,HIGH);
      digitalWrite(column2,LOW);
      digitalWrite(column2,HIGH);
      break;       
  }
}

void set_row(int row) {
  switch (row) {
    case 0:
      digitalWrite(row0,LOW);
      digitalWrite(row1,LOW);
      digitalWrite(row2,LOW);
      break;
    case 1:
      digitalWrite(row0,HIGH);
      digitalWrite(row1,LOW);
      digitalWrite(row2,LOW);
      break;
    case 2:
      digitalWrite(row0,LOW);
      digitalWrite(row1,HIGH);
      digitalWrite(row2,LOW);
      break;
    case 3:
      digitalWrite(row0,HIGH);
      digitalWrite(row1,HIGH);
      digitalWrite(row2,LOW);
      break;
    case 4:
      digitalWrite(row0,LOW);
      digitalWrite(row1,LOW);
      digitalWrite(row2,HIGH);
      break;
    case 5:
      digitalWrite(row0,HIGH);
      digitalWrite(row1,LOW);
      digitalWrite(row2,HIGH);
      break;
    case 6:
      digitalWrite(row0,LOW);
      digitalWrite(row1,HIGH);
      digitalWrite(row2,HIGH);
      break;
    case 7:
      digitalWrite(row0,HIGH);
      digitalWrite(row1,HIGH);
      digitalWrite(row2,HIGH);
      break;      
  }
}
boolean LIS3DH_init(void) {
        if(!lis.begin(0x18)) {
                Serial.printf("Couldn't start!\n");
                return 0;
        }
        Serial.printf("LIS3DH Found");

        lis.setRange(LIS3DH_RANGE_4_G);

        Serial.printf("Range = %d G \n",lis.getRange());
        return 1;
}

void LIS3DH_sendData(void) {
        Serial.printf("%f,%f,%i,",accel_data.pitch,accel_data.roll,!digitalRead(button));
}

void LIS3DH_getData(void) {
        //REMINDER, I2C Pins SDA1==D0, SCL1 == D1
        // get X Y and Z data at once
        lis.read();
        //get a new sensor event, normalized
        sensors_event_t event;
        lis.getEvent(&event);
        //Get the results (acceleration measured in m/s^2)
        accel_data.xG = (event.acceleration.x/(9.8));
        accel_data.yG = (event.acceleration.y/(9.8));
        accel_data.zG = (event.acceleration.z/(9.8));

        accel_data.pitch = atan(accel_data.xG/sqrt(pow(accel_data.yG,2) + pow(accel_data.zG,2)));
        accel_data.roll = atan(accel_data.yG/sqrt(pow(accel_data.xG,2) + pow(accel_data.zG,2)));

        accel_data.pitch = accel_data.pitch * (180/PI);
        accel_data.roll = accel_data.roll * (180/PI);
}


