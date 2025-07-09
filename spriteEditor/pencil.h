/**
 * Declaration of the Pencil class, which functions as a tool.
 *
 * @author Pierce Jones
 * @date March 31, 2025
 * @reviewer Golightly Chamberlain
 */

#ifndef PENCIL_H
#define PENCIL_H

#include <QColor>
#include <QImage>
#include <QPainter>

/**
 * The Pencil class captures the basic attributes of a pencil,
 * such as its length and color, and offers functionality to
 * simulate using the pencil.
 */
class Pencil
{
public:

    /**
     * @brief Constructs a Pencil object with the specified size and color.
     * @param size The initial size of the pencil tip (default is 1).
     * @param color The initial QColor of the pencil (default is Qt::black).
     */
    Pencil(int size = 1, QColor color = Qt::black);

    /**
     * @brief Sets the size used for drawing.
     * @param size The new pencil drawing size.
     */
    void setPencilSize(int size);

    /**
     * @brief Sets the size used for erasing.
     * @param size The new eraser size.
     */
    void setEraserSize(int size);

    /**
     * @brief Updates the color of the pencil.
     * @param color The new pencil color (QColor).
     */
    void setColor(const QColor &color);

    /**
     * @brief Retrieves the current size of the active tool
     *        (pencil if in drawing mode, eraser if in eraser mode).
     * @return The size of the active tool.
     */
    int getSize() const;

    /**
     * @brief Draws at the specified location on the provided canvas.
     * @param mouseX The X coordinate where drawing should occur.
     * @param mouseY The Y coordinate where drawing should occur.
     * @param canvas The QImage on which drawing takes place.
     * @param canvasDivisions A division factor (for scaling or offset)
     *        applied to the mouse coordinates.
     */
    void draw(float mouseX, float mouseY, QImage& canvas, int canvasDivisions);

    /**
     * @brief Erases at the specified location on the provided canvas.
     * @param mouseX The X coordinate where erasing should occur.
     * @param mouseY The Y coordinate where erasing should occur.
     * @param canvas The QImage on which erasing takes place.
     * @param canvasDivisions A division factor (for scaling or offset)
     *        applied to the mouse coordinates.
     */
    void erase(float mouseX, float mouseY, QImage& canvas, int canvasDivisions);

    /**
     * @brief Sets the tool mode to pen (drawing).
     */
    void setToPen();

    /**
     * @brief Sets the tool mode to eraser.
     */
    void setToEraser();

    /**
     * @brief Retrieves the current mode of the pencil.
     * @return True if in drawing mode, false if in eraser mode.
     */
    bool getMode();

    /**
     * @brief Retrieves the current color of the pencil.
     * @return The QColor representing the pencil’s color.
     */
    QColor getColor() const;

private:
    int pencilSize;        /// Size of the pencil tip in drawing mode
    int eraserSize;        /// Size of the eraser in eraser mode
    bool isDrawing = true; /// True if the pencil is in drawing mode, false if erasing
    QColor pencilColor;    /// Current color of the pencil
};

#endif
