/**
 * Implementation of the Sprite Class
 *
 * @authors Matt Shaw, Golightly Chamberlain, Cheuk Yin Lau
 * @date March 31, 2025
 * @reviewer Hudson Dalby
 */

#include "sprite.h"

Sprite::Sprite(int size, QWidget *parent)
    : QLabel(parent)
    , spriteSize(size)
{
    setSpriteSize(size);
}

Sprite::Sprite(const Sprite &other)
    : QLabel(nullptr)
    , image(other.image.copy())
    , spriteSize(other.spriteSize)
{

}

Sprite::~Sprite()
{

}

void Sprite::setSpriteSize(int newSize)
{
    spriteSize = newSize;
    image = QImage(spriteSize, spriteSize, QImage::Format_ARGB32);
    image.fill(Qt::transparent);
}

int Sprite::getSpriteSize() const
{
    return spriteSize;
}

const QImage &Sprite::getImage() const
{
    return image;
}

void Sprite::setImage(const QImage &newImage)
{
    if (newImage.isNull())
    {
        image = QImage(2, 2, QImage::Format_ARGB32);
        image.fill(Qt::transparent);
        spriteSize = 2;
    }
    else
    {
        image = newImage.copy();
        spriteSize = image.width();
    }
}

QColor Sprite::getPixel(int x, int y) const
{
    if (x >= 0 && x < spriteSize && y >= 0 && y < spriteSize)
    {
        return image.pixelColor(x, y);
    }
    else
    {
        return QColor(Qt::transparent);
    }
}

void Sprite::setPixel(int x, int y, const QColor &color)
{
    if (x >= 0 && x < spriteSize && y >= 0 && y < spriteSize)
    {
        image.setPixelColor(x, y, color);
    }
}

void Sprite::setPencil(Pencil *pencil)
{
    this->pencil = pencil;
}

void Sprite::setQFrame(QFrame *frame)
{
    setParent(frame);

    if (!frame)
    {
        return;
    }

    frame->show();
    frameWidth = frame->width();
    frameHeight = frame->height();
}

void Sprite::undoPaint()
{
    if (!undoStack.empty())
    {
        redoStack.push(image);
        image = undoStack.pop();    // Sets current image to previous undo stack element
        update();
    }
}

void Sprite::redoPaint()
{
    if (!redoStack.empty())
    {
        undoStack.push(image);
        image = redoStack.pop();    // Sets current image to previous redo stack element
        update();
    }
}

bool Sprite::getEyedropperEnabled(){
    return eyeDropperEnabled;
}

void Sprite::setEyedropper(bool active){
    eyeDropperEnabled = active;
}

void Sprite::paintEvent(QPaintEvent *)
{
    QPainter painter(this);
    painter.drawImage(rect(), getImage());  // Draws the sprite as a QImage and emits update
    emit spriteUpdated();
}

void Sprite::mousePressEvent(QMouseEvent *event)
{
    QWidget *focused = QApplication::focusWidget(); // Focuses the mouse press within the QFrame canvas
    if (focused && focused != this)
    {
        focused->clearFocus();
    }

    QPoint pt = mousePos2Px(event -> pos());
    if (eyeDropperEnabled)
    {
        emit changeColor(image.pixelColor(pt)); // Eyedropper tool changes color and deactivates
        setEyedropper(false);
        emit disableDropper();
    }
    else
    {
        undoStack.push(image);  // Handles stack data for undo and redo
        redoStack.clear();
        if (pencil->getMode())  // determines the current tool mode
        {
            pencil->draw(pt.x(), pt.y(), image, spriteSize);
        }
        else
        {
            pencil->erase(pt.x(), pt.y(), image, spriteSize);
        }
    }
    update();
}

void Sprite::mouseMoveEvent(QMouseEvent *event)
{
    if (event->buttons() & Qt::LeftButton) 
    {
        QPoint pt = mousePos2Px(event->pos());  // Gets current position of the mouse
        if (pencil->getMode())
        {
            pencil->draw(pt.x(), pt.y(), image, spriteSize);
        }
        else
        {
            pencil->erase(pt.x(), pt.y(), image, spriteSize);
        }
        update();
    }
}

QPoint Sprite::mousePos2Px(QPoint pt)
{
    QSize labelSize = size();
    int x = pt.x() * spriteSize / labelSize.width();    // Gets x value of position within frame
    int y = pt.y() * spriteSize / labelSize.height();   // Gets y value of position within frame
    return QPoint(x, y);
}
