import processing.sound.*;

PGraphics target;
PGraphics board;
Animation animation0,animation1, animation2,animation3;

import processing.serial.*;                  // import the serial lin
//import processing.sound.*;


//variables from serial port (coming from RedBear Duo)
float pitch, roll; 
int feetsensor;                           
float target_pitch, target_roll;
float new_target_pitch, new_target_roll;
float positionx, positiony;                              // position to translate to
int state = 0;
Serial myPort;                               // the serial port
PImage wave;
PFont start_text;
PFont nailed_it_text;
PFont play_text;
boolean new_target = true;
boolean holding = false;
boolean nailed_it = false;
float eps = 20;
float tintr = 255, tintb = 255, tintg = 255;
PShape SurfBoard;
int hold = 2000;
int hold_time, time;
boolean moving_roll = true, moving_pitch = true;
String[] nailed_it_strings = {"Nailed It!", "Radical!", "Get Pitted!", "Gnarly!", "Killing It!"};
int num_string = 5;
int i = 0, sound = 0;
boolean printed_text = false;

//sound declaration
SoundFile ding, surf_music;
SoundFile wack, so_pitted, bro, whoo, so_sick;
int sound_length = 4;


int start_time, countdown;
boolean first_time = true;
String countdown_string[] = {"3","2","1"};
int count_index = 0;

//countdown and score
PFont font, font2;
int t, t2;
int lastTime=0;
String time2 = "30";
String timeA = "05";
int interval=7;
int interval2=30;
int score=0;
boolean nextTime=false;

//SoundFile nailed_it_sound[2];

void setup() {
  size(736, 552, P3D); 
  frameRate(15);
  animation0 = new Animation("catstep/tmp-", 38);
  animation1 = new Animation("tuberiding/tmp-", 183);
  animation2 = new Animation("wipeout/tmp-", 31);
  animation3 = new Animation("surfrelax/tmp-", 30);
  
  //sound stuff
  ding = new SoundFile(this,"ding.mp3");
  surf_music = new SoundFile(this,"surf_music.mp3");
  wack = new SoundFile(this,"wack.mp3");
  so_pitted = new SoundFile(this,"so_pitted.mp3");
  bro = new SoundFile(this,"bro.mp3");
  whoo = new SoundFile(this,"whoo.mp3");
  so_sick = new SoundFile(this,"so_sick.mp3");
  surf_music.loop();
  //nailed_it_sound[1] = new SoundFile(this,"so_pitted.mp3");
  
  target = createGraphics(width, height, P3D);
  board = createGraphics(width, height, P3D);
  wave = loadImage("wave_2.jpg");

  myPort = new Serial(this, Serial.list()[2], 9600);
  // only generate a serial event when you get a newline: 
  myPort.bufferUntil('\n');
  SurfBoard = loadShape("SurfBoard.obj");

  //SurfBoard = loadShape("customboard.obj");
  //SurfBoard = loadShape("smallboard.obj");
} 

void draw() {
  //background(wave);
  
  //state machine for the game
  switch(state) {
  case 0: //Waiting for player to stand over board
    animation0.display(0, 0);
    if (feetsensor==1) {
      
      if(first_time) {
        start_time = millis();
        print("first press");
        first_time = false;
      }
      
      countdown = millis();
      
      //t = interval-(int(start_time/1000));
      
      //timeA = nf(t , 2);
      
      if(((start_time - countdown) <  1000)){ // && count_index < 2 ){
            print("Inside countdown");
             text(countdown_string[count_index], width/2.3, height/1.8);
             count_index++;
      }
      else if(count_index > 2){
        text("GO", width/2.3, height/1.8);
        delay(500);
        nextTime=true;
        lastTime=millis();
     
       //play();
        first_time = true;
        play();
        state=1;
      }
    } else {
      start_screen();
    }
    break;
  case 1: //player playing
    animation1.display(0, 0);
    if (feetsensor==0) {
      start_screen();
      state=3;
    } else {
      play();
    }
    break;
  case 3: //wipeout
    animation2.display(0, 0);
    if (feetsensor==1) {
      play();
      state=4;
    }     
    break;
  case 4: //transition state before game goes back to initial page, player must step up and down once, (this can be implemented with a timer)
    animation2.display(0, 0);
    if (feetsensor==0) {
      state=0;
    }     
    break;
  default:
    println("default");   // Does not execute
    break;
  }
}

void start_screen() {
   
  start_text = createFont("chalkboard", 100, true);
  textFont(start_text, 36);
  fill(0xFF000000);
  textAlign(CENTER);
  text("Stand on Board to Begin!", width/2, height/10);
}

void play() {
  if (new_target) {
    // translate from origin to center: 
    new_target_roll = random(-30, 30);
    new_target_pitch = random(-30, 30);
    new_target = false;
  }

  drawtarget();
  drawboard();

  tint(255, 255);
  image(target, 0, 0);
  //tint(board_tint, 128);
  tint(tintr, tintg, tintb, 128);
  image(board, 0, 0);
  if ((moving_roll || moving_pitch) && nailed_it) {
    nailed_it_text = createFont("chalkboard", 100, true);
    textFont(nailed_it_text, 36);

    //translate(width/2,8*height/10);
    textAlign(CENTER);
    fill(0xFFFF0000);

    if (!printed_text) {
      i++;
      if (i>=num_string)
        i = 0;
    }
    printed_text = true;
    text(nailed_it_strings[i], width/2, 8*height/10);
  }
}

void drawtarget() {
  target.beginDraw();
  target.lights();
  target.clear();
  target.noStroke();
  target.translate(width/2, height/2);
  moving_roll = true;
  moving_pitch = true;

  //this is to go step by step, TODO: change speed by changing 0.5 criterium?
  if (abs(new_target_roll - target_roll) < 0.5) {
    print("stopped rolling \n");
    moving_roll = false;
  } else if (new_target_roll > target_roll) {
    target_roll += 0.5;
  } else if (new_target_roll < target_roll) {
    target_roll -= 0.5;
  }

  if (abs(new_target_pitch - target_pitch) < 0.5) {
    print("Stopped pitching \n");
    moving_pitch = false;
  } else if (new_target_pitch > target_pitch) {
    target_pitch += 0.5;
  } else if (new_target_pitch < target_pitch) {
    target_pitch -= 0.5;
  }


  target.rotateX(radians(target_roll + 90));
  target.rotateY(radians(target_pitch));
  target.rotateZ(radians(90));


  target.shape(SurfBoard);
  target.endDraw();
}

void drawboard() {
  board.beginDraw();
  board.lights();
  board.clear();
  board.noStroke();
  board.translate(width/2, height/2);

  board.rotateX(radians(roll + 90));
  board.rotateY(radians(pitch) );
  board.rotateZ(radians(90));

  /*
  board.rotateX(radians(roll + 90));
   board.rotateZ(radians(pitch) );
   */
  if ((abs(pitch - target_pitch) < eps) && abs(roll - target_roll) < eps && !moving_pitch && !moving_roll) { 
    if (!holding) {
      hold_time = millis();
      print("start hold");
    }
    holding = true;
    tintr = 0;
    tintb = 145;
    tintg = 255;
  } else {
    holding = false;
    tintr = 255;
    tintb = 255;
    tintg = 255;
  }
  //board.ellipse(0, 0, width/2, width/2); 
  board.shape(SurfBoard);
  board.endDraw();
  time = millis();
  if ((time - hold_time >= hold) && holding) {
    print("holding done");
    new_target = true;
    nailed_it = true;
    printed_text = false;
    
    //sound stuff
    ding.play();
    if(sound == 0)
      wack.play();
      else if(sound == 1)
        so_pitted.play();
              else if(sound == 2)
                bro.play();
                else if(sound == 3)
                  whoo.play();
                  else if(sound == 4)
                    so_sick.play();
    sound++;
    if(sound > sound_length)
      sound = 0;
    /*
    nailed_it_sound[sound].play();
    sound++;
    if(sound > nailed_it_sound.length)
      sound = 0;
      */
  }
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
  }
}