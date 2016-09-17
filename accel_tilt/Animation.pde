// Class for animating a sequence of GIFs

class Animation {
  PImage[] images;
  PImage imageaux;
  int imageCount;
  int frame;
  
  Animation(String imagePrefix, int count) {
    imageCount = count;
    images = new PImage[imageCount];
    

    for (int i = 0; i < imageCount; i++) {
      // Use nf() to number format 'i' into four digits
      String filename = imagePrefix + nf(i) + ".gif";
      images[i] = loadImage(filename);
    }
  }

  void display(float xpos, float ypos) {
    frame = (frame+1) % imageCount;
    image(images[frame], xpos, ypos,736, 552);

  }
  
  int getWidth() {
    return images[0].width;
  }
}