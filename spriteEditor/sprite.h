/**
 * This class represents a sprite object. It is the collection of pixels of a specified size
 * that are able to be manipulated through the pencil object. This class handles mouse position
 * and events that occur on the editor canvas.
 *
 * @authors Matt Shaw, Golightly Chamberlain, Cheuk Yin Lau
 * @date March 31, 2025
 * @reviewer Hudson Dalby
 */

#ifndef SPRITE_H
#define SPRITE_H

#include <QImage>
#include <QLabel>
#include <QMouseEvent>
#include <QObject>
#include <QPainter>
#include <QStack>
#include "pencil.h"
#include <QApplication>

/*
 * The Sprite class is defined as a subclass of the QLabel class for integration with QFrame
 * elements in the editor UI. The primary manipulation of pixel data within the sprite class happens
 * here, where methods are called through tools
 */
class Sprite : public QLabel
{
    Q_OBJECT

public:

    /**
     * @brief Sprite constructor.
     * @param size of the sprite.
     * @param parent widget of the Sprite.
     */
    explicit Sprite(int size = 2, QWidget *parent = nullptr);

    /**
     * @brief Sprite Copy constructor for Sprite.
     * @param other - The other Sprite from which to copy.
     */
    Sprite(const Sprite &other);

    /**
     * @brief Default constructor for the Sprite Class.
     */
    ~Sprite();

    /**
     * @brief setSpriteSize Sets the sprite size to the given width/height. Only takes one param because
     * sprite is always assumed to be square.
     * @param newSize - The new side length of the sprite.
     */
    void setSpriteSize(int newSize);

    /**
     * @brief getSpriteSize Gets the side length of the sprite.
     * @return Returns an int representing the side length of the sprite.
     */
    int getSpriteSize() const;

    /**
     * @brief getImage Gets the entire image that the Sprite contains.
     * @return A QImage representing the entire image that the sprite contained.
     */
    const QImage &getImage() const;

    /**
     * @brief setImage Sets the entire Sprite to a new image.
     * @param image The image that will be set on the Sprite.
     */
    void setImage(const QImage &image);

    /**
     * @brief getPixel Gets a particular pixel on the Sprite.
     * @param x The x (width) position of the pixel.
     * @param y The y (height) position of the pixel.
     * @return returns the QColor value of a pixel
     */
    QColor getPixel(int x, int y) const;

    /**
     * @brief setPixel Sets a particular pixel on the sprite.
     * @param x - The x (width) position of the pixel.
     * @param y - The y (height) position of the pixel.
     * @param color - the QColor data of the pixel.
     */
    void setPixel(int x, int y, const QColor &color);

    /**
     * @brief setPencil Sets the pencil to be used on the sprite.
     * @param pencil The pencil object.
     */
    void setPencil(Pencil *pencil);

    /**
     * @brief setQFrame
     * @param frame
     */
    void setQFrame(QFrame *frame);

    /**
     * @brief toggleEyedropper Toggles whether the eyedropper is set to true or not.
     */
    void toggleEyedropper();

    /**
     * @brief getEyedropperEnabled returns whether the eyedropper is set to true/false
     */
    bool getEyedropperEnabled();

    /**
     * @brief setEyedropper sets the eyedropper to the parameter state.
     * @param active - whether dropper is active or not.
     */
    void setEyedropper(bool active);

public slots:

    /**
     * @brief pops a frame from the undo stack to set as the current image.
     */
    void undoPaint();

    /**
     * @brief pops a frame from the redo stack to set as the current image.
     */
    void redoPaint();

private:

    // The stored mouse position in relaton to the sprite.
    QPoint mousePos2Px(QPoint pt);

    // The entire image that represents the sprite.
    QImage image;

    // The stack that holds QImages for the undo button functionality.
    QStack<QImage> undoStack;

    // The stack that holds QImages for the redo button functionality.
    QStack<QImage> redoStack;

    // A reference to the pencil object used in sprite manipulation.
    Pencil *pencil;

    // The size of the sprite represented by it's side lengths.
    int spriteSize;

    // The width of the frame in the UI.
    int frameWidth;

    // The height of the frame in the UI.
    int frameHeight;

    // The state of the eyedropper being enabled.
    bool eyeDropperEnabled = false;

protected:

    /**
     * @brief represents the current event that alters the sprite canvas.
     * Overload of the default QPaintEvent method, paintEvent.
     * @param event - the current process of pixel manipulation.
     */
    void paintEvent(QPaintEvent *event) override;

    /**
     * @brief represents the current state of user clicking the mouse.
     * @param event - the current updates of the mouse being clicked.
     */
    void mousePressEvent(QMouseEvent *event) override;

    /**
     * @brief represents the movement of the user's mouse.
     * @param event - the update of the mouse position changing.
     */
    void mouseMoveEvent(QMouseEvent *event) override;

signals:

    /**
     * @brief signals a color change of pixel data.
     * @param color - color of the updated pixel.
     */
    void changeColor(const QColor &color);

    /**
     * @brief signals the sprite data has been updated.
     */
    void spriteUpdated();

    /**
     * @brief signals the dropper has been used for a one-time action.
     */
    void disableDropper();
};

#endif // SPRITE_H
