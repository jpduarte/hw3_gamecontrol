// This example code is in the public domain.

import processing.serial.*;
import processing.opengl.*;


int bgcolor;                 // Background color
int fgcolor;                 // Fill color
Serial myPort;                       // The serial port
int[] serialInArray = new int[225];    // Where we'll put what we receive
int[] pastInArray = new int [225];
float[][] colorTarget   = new float[3][255];
float[][] currentColor   = new float[3][255];
PVector[][] vertices = new PVector[15][15];
float[] verticesTZ = new float[15];
float w = 30;
float ease = 0.75; 

int serialCount = 0;                 // A count of how many bytes we receive
int xpos, ypos;                  // Starting position of the ball
boolean firstContact = false;        // Whether we've heard from the microcontroller
int tiempoant;
int render=0;
int dif=0;

//
float pitch=0, roll=0; 
int feetsensor=0;  

void setup() {
  size(960, 600, OPENGL);  // Stage size
  noStroke();      // No border on the next thing draw
  
  
  // Print a list of the serial ports, for debugging purposes:
  println(Serial.list());

  // I know that the first port in the serial list on my mac
  // is always my  FTDI adaptor, so I open Serial.list()[0].
  // On Windows machines, this generally opens COM1.
  // Open whatever port is the one you're using.
  

  myPort = new Serial(this, Serial.list()[0], 9600);
  myPort.bufferUntil('\n');
  
  for (int j = 0; j < 8; j++) {
        for (int i = 0; i < 12; i++) {
            vertices[i][j] = new PVector( i*w, j*w, 0);
        }
    }
    
  
}

void draw() {
    //println
    translate(width/4, height/4);
    rotateX(0.5);
    //rotateX(PI/10);
    background(0);
     
    if (render==1){
      //println("Printing");
    int index=0;
    //loop row first
    for (int rowcount = 0;rowcount<8;rowcount++) {
       //then loop columns
       for (int columncount = 0;columncount<12;columncount++) {
        vertices[columncount][rowcount].z=serialInArray[index]/40;//+= (serialInArray[index]/40-vertices[columncount][rowcount].z)*ease; 
        index=index+1;
       }
    } 
    
   for (int j=0; j<7; j++) {
      beginShape(QUAD_STRIP);
      for (int i=0; i<12; i++) {
          stroke(255);
          float colorfill =  (vertices[i][j].z)*255.0*40.0/4095.0;//((vertices[i][j].z+vertices[i][j+1].z+ vertices[i+1][j].z+vertices[i+1][j+1].z)*255.0*40/4095.0)/4.0;
          fill(colorfill, 0, 0);
          //println(colorfill);
          //+= (verticesTZ[i]-vertices[i][j].z)*ease;
          vertex( vertices[i][j].x, vertices[i][j].y, vertices[i][j].z);
          vertex( vertices[i][j+1].x, vertices[i][j+1].y, vertices[i][j+1].z);
          //vertex( vertices[i+1][j].x, vertices[i+1][j].y, vertices[i+1][j].z);
          //vertex( vertices[i+1][j+1].x, vertices[i+1][j+1].y, vertices[i+1][j+1].z);          
        }
         endShape(CLOSE);
        //        println();
      }    
      
    } 
    render=0;

}

void serialEvent(Serial myPort) {
  // read the serial buffer:
  String myString = myPort.readStringUntil('\n');
  // if you got any bytes other than the linefeed: 
  if (myString != null) {
    myString = trim(myString);
    String items[] = split(myString, ','); 
    pitch = float(items[0]);
    roll = float(items[1]);
    feetsensor = int(items[2]);
    int index=3;
    //loop row first
    for (int rowcount = 0;rowcount<8;rowcount++) {
       //then loop columns
       for (int columncount = 0;columncount<12;columncount++) {
        serialInArray[index-3]=int(items[index]); 
        //vertices[rowcount][columncount].z=int(items[index])/40; 
        index=index+1;
        }   
    }
  render=1;  
  }
}