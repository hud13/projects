/**
 * Implementation of the Pencil class, which functions as a tool.
 *
 * @author Pierce Jones
 * @date March 31, 2025
 * @reviewer Golightly Chamberlain
 */

#include "pencil.h"

Pencil::Pencil(int size, QColor color)
    : pencilSize(size)
    , eraserSize(size)
    , pencilColor(color)
{
    isDrawing = true;
}

void Pencil::setPencilSize(int size)
{
    pencilSize = size;
}

void Pencil::setEraserSize(int size)
{
    eraserSize = size;
}

void Pencil::setColor(const QColor &color)
{
    pencilColor = color;
}

int Pencil::getSize() const
{
    if (isDrawing) {
        return pencilSize;
    } else {
        return eraserSize;
    }
}

QColor Pencil::getColor() const
{
    return pencilColor;
}

void Pencil::setToEraser()
{
    isDrawing = false;
}

void Pencil::setToPen()
{
    isDrawing = true;
}

bool Pencil::getMode()
{
    return isDrawing;
}

void Pencil::draw(float mouseX, float mouseY, QImage &canvas, int canvasDivisions)
{
    if (isDrawing) {
        int pixelWidth = canvas.width() / canvasDivisions;

        int pixelX = mouseX / pixelWidth;
        int pixelY = mouseY / pixelWidth;

        int middleX = pixelX * pixelWidth;
        int middleY = pixelY * pixelWidth;

        int offset = (pencilSize - 1) / 2;

        int startSquareX = middleX - (offset * pixelWidth);
        int startSquareY = middleY - (offset * pixelWidth);

        QPainter painter(&canvas);
        painter.setCompositionMode(QPainter::CompositionMode_Source);
        painter.fillRect(startSquareX,
                         startSquareY,
                         pixelWidth * pencilSize,
                         pixelWidth * pencilSize,
                         pencilColor);
    }
}

void Pencil::erase(float mouseX, float mouseY, QImage &canvas, int canvasDivisions)
{
    if (!isDrawing) {
        int pixelWidth = canvas.width() / canvasDivisions;

        int pixelX = mouseX / pixelWidth;
        int pixelY = mouseY / pixelWidth;

        int middleX = pixelX * pixelWidth;
        int middleY = pixelY * pixelWidth;

        int offset = (eraserSize - 1) / 2;

        int startSquareX = middleX - (offset * pixelWidth);
        int startSquareY = middleY - (offset * pixelWidth);

        QPainter painter(&canvas);
        painter.setCompositionMode(QPainter::CompositionMode_Source);
        painter.fillRect(startSquareX,
                         startSquareY,
                         pixelWidth * eraserSize,
                         pixelWidth * eraserSize,
                         QColor(0, 0, 0, 0));
    }
}
