/*
Surf sim 
acceleroeter Redbear Duo code
*/
 
// do not use the cloud functions - assume programming through Arduino IDE
#if defined(ARDUINO) 
SYSTEM_MODE(MANUAL); 
#endif
// Basic demo for accelerometer readings from Adafruit LIS3DH

#include "Adafruit_LIS3DH.h"
#include "Adafruit_Sensor.h"

#define PI 3.1415

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
  
  LIS3DH_init();
}

void loop() {
  
  LIS3DH_getData();
  LIS3DH_sendData();
 
  delay(33); //33Hz update rate
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

void LIS3DH_printXYZ(void) {
        // get X Y and Z data at once
        lis.read();

        // print out raw data
        //Serial.printf("Raw Data: \tX: %d \tY: %d \tZ: %d \n",lis.x,lis.y,lis.z);

        //get a new sensor event, normalized
        sensors_event_t event;
        lis.getEvent(&event);

        // Display the results (acceleration measured in m/s^2)
        Serial.printf("Acceleration: \tX: %f m/s^2 \tY: %f m/s^2 \tZ: %f m/s^2 \n",(event.acceleration.x/(9.8)),(event.acceleration.y/(9.8)),(event.acceleration.z/(9.8)));

}

void LIS3DH_sendData(void) {
  
        // Display the results (acceleration measured in m/s^2)
        /*
        Serial.printf("%f\n",accel_data.xG);
        Serial.printf("%f\n",accel_data.yG);
        Serial.printf("%f\n",accel_data.zG);
        */

        Serial.printf("%f,%f\n",accel_data.pitch,accel_data.roll);
}

void LIS3DH_getData(void) {
        // get X Y and Z data at once
        lis.read();

        // print out raw data
        //Serial.printf("Raw Data: \tX: %d \tY: %d \tZ: %d \n",lis.x,lis.y,lis.z);

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
