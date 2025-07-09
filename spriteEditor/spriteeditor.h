/**
 * A Sprite Editor program that allows users to paint pixels on a surface and create
 * animated sprites from multiple frames.
 *
 * @author Golightly Chamberlain, Pierce Jones, Hudson Dalby, Matthew Shaw, Cheuk Yin Lau, and John Gibb
 * @date March 31, 2025
 * @reviewer John Gibb and Hudson Dalby and Pierce Jones
 */

#ifndef SPRITEEDITOR_H
#define SPRITEEDITOR_H

#include <QColor>
#include <QFileDialog>
#include <QLabel>
#include <QMainWindow>
#include <QPainter>
#include <QPalette>
#include <QPixmap>
#include <QShortcut>
#include <QTimer>
#include "pencil.h"
#include "spritemodel.h"
#include <QDebug>
#include "startmenu.h"
#include "ui_spriteeditor.h"
#include "playbackwindow.h"


QT_BEGIN_NAMESPACE
namespace Ui {
class SpriteEditor;
}
QT_END_NAMESPACE

class SpriteEditor : public QMainWindow
{
    Q_OBJECT
public:
    /**
     * @brief SpriteEditor Constructor for this sprite editor application. Initializes
     * the ui, model, pencil, and playback windows. Connect the ui elements to functions.
     * @param model The model of this sprite editor application.
     * @param parent The parent class QWidget.
     */
    SpriteEditor(SpriteModel *model, QWidget *parent = nullptr);

    /**
     * @brief ~SpriteEditor Deconstructs the instance variables that are initialized
     * on the heap.
     */
    ~SpriteEditor();

private slots:

    /**
     * @brief backToMainMenu Returns the user to the main menu window.
     */
    void backToMainMenu();

    /**
     * @brief saveProject Saves this sprite editor project to a .ssp file.
     */
    void saveProject();

    /**
     * @brief updateColorSelector Updates the color selector slider values.
     * @param color The color to represent in the color selector sliders.
     */
    void updateColorSelector(const QColor &color);

    /**
     * @brief updateColorPreview Updates the preview box with the color
     * represented by the color selector sliders.
     */
    void updateColorPreview();

    /**
     * @brief updatePenSizeLabel Updates the pen size label.
     * @param size The size of the pen.
     */
    void updatePenSizeLabel(int size);

    /**
     * @brief penSizeChanged Changes the pen size when the slider is moved.
     */
    void penSizeChanged();

    /**
     * @brief penSelected Sets the user's cursor to pen mode. Enables drawing.
     */
    void penSelected();

    /**
     * @brief updateEraserSizeLabel Updates the eraser size label.
     * @param size The size of the eraser.
     */
    void updateEraserSizeLabel(int size);

    /**
     * @brief eraserSizeChanged Changes the eraser size when the slider is moved.
     */
    void eraserSizeChanged();

    /**
     * @brief eraserSelected Sets the user's cursor to eraser mode.
     * Enables erasing pixels.
     */
    void eraserSelected();

    /**
     * @brief Start or stop sprite animation preview
     */
    void toggleAnimation();

    /**
     * @brief Let the animation box show the next frame
     */
    void nextAnimationFrame();

    /**
     * @brief eyedropperToggled Sets the user's cursor to the
     * eyedropper state. Captures the color value of the pixel
     * the user clicks on next.
     * @param checked Boolean indicating if the user's cursor
     * is in the eyedropper state.
     */
    void eyedropperToggled(bool checked);

    /**
     * @brief dropperFinished Toggles the eyedropper button's
     * visual representation indicating what state the eyedropper
     * is in.
     */
    void dropperFinished();

    /**
     * @brief handleDropper Handler for when the eyedropper button is
     * clicked.
     */
    void handleDropper();

    /**
     * @brief handleAdd Handler for when the add frame button is clicked.
     * Adds a blank frame.
     */
    void handleAdd();

    /**
     * @brief handleCopy Handler for when the copy button is clicked.
     * Copies the current frame.
     */
    void handleCopy();

    /**
     * @brief handlePaste Handler for when the paste button is clicked.
     * Pastes the frame in the clipboard after the current frame.
     */
    void handlePaste();

    /**
     * @brief handleUndo Handler for when the undo button is clicked.
     * Reverts the previous paint action.
     */
    void handleUndo();

    /**
     * @brief handleRedo Handler for when the redo button is clicked.
     * Reverts the previous undo action.
     */
    void handleRedo();

public slots:

    /**
     * @brief Function to update the FPS of the animation preview playback.
     * @param fps - the updated playback speed.
     */
    void updateFPS(int fps);

    /**
     * @brief Function to delete the frame preview from the frame menu.
     */
    void deleteFromFrameListWidget();

    /**
     * @brief Function to update the appearance of the frame icons in the frame menu.
     */
    void updateFrameIcons();

    /**
     * @brief Function to switch a frame in the preview with the provided frame.
     * @param frame - the frame to switch the current with.
     */
    void switchFrame(QListWidgetItem *frame);

    /**
     * @brief Function that focuses mouse events on the sprite frame, allowing edits to be made
     *        to the pixel data through tools and other manipulation.
     * @param sprite - The sprite in the frame to receive the focus.
     */
    void frameFocus(Sprite *sprite);

private:
    /**
     * @brief Show the given image in given widget using Pixmap and QLabel.
     * @param img - Image to been shown.
     * @param box - Widget to show the image.
     */
    void showImg(QImage img, QWidget *box);

    /**
     * @brief the view instance of the editor.
     */
    Ui::SpriteEditor *ui;

    /**
     * @brief the model instance of the editor.
     */
    SpriteModel *model;

    /**
     * @brief a reference to the current sprite being edited.
     */
    Sprite *currentDrawingSprite;

    /**
     * @brief a reference to the pencil object being used to manipulate sprite pixels.
     */
    Pencil *pencil;

    /**
     * @brief a timer used to control the speed of the playback animation.
     */
    QTimer *animationTimer;

    /**
     * @brief The position of the current frame in the editor and preview menu.
     */
    int currentFrameIndex = 0;

    /**
     * @brief The position of the current frame in the playback sequence.
     */
    int currentAnimationFrameIndex = 0;
};

#endif // SPRITEEDITOR_H
