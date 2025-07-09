/**
 * Handles spritie project state. Manages frames, clipboard actions, and undo/redo history. Handles save / load from JSON.
 *
 * @author John Gibb
 * @date March 31, 2025
 * @reviewer Matthew Shaw
 */

#ifndef SPRITEMODEL_H
#define SPRITEMODEL_H

#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QListWidgetItem>
#include <QObject>
#include "sprite.h"
#include <stack>

/**
 * @brief The SpriteModel class represents a model for managing a sprite object.
 * It allows for loading / saving project data, manipulating frames, and signals updates to the view end.
 */
class SpriteModel : public QObject
{
    Q_OBJECT
public:
    /**
     * @brief SpriteModel Constructs a SpriteModel with an initial default frame.
     * @param defaultSize The pixel size of the default frame.
     * @param parent Optional QObject parent.
     */
    explicit SpriteModel(int defaultSize = 2, QObject *parent = nullptr);

    /**
     * @brief loadProject Loads a given Json into a Sprite.
     * @param fileName The path where the .ssp project file will be saved.
     */
    void loadProject(const QString &fileName);

    /**
     * @brief saveProject Saves the Sprite to a json object.
     * @param fileName THe path of .ssp project file.
     */
    void saveProject(const QString &fileName);

    /**
     * @brief copy Adds the frame at the given position in the sprite frame list to the clipboard. For example, if
     * 3 is given, the fourth frame in the sprite is added to the clipboard.
     * @param frame is the frame position from which to get the frame and add it to the clipboard. **Expects 0 indexed frames**
     */
    void copy(int frame);

    /**
     * @brief paste Adds the current frame on clipboard to the frame one position ahead of the currentFrame param.
     * @param currentFrame The frame which will be immediately before the pasted frame.
     */
    void paste(int currentFrame);

    /**
     * @brief addFrame Adds an empty frame to the end of the frame list.
     */
    void addFrame(int currentFrame);

    /**
     * @brief deleteFrame Deletes the given frame from the Sprite.
     * @param framePos The position of the frame to be deleted. Expects 0 indexed frames.
     */
    void deleteFrame(int framePos);

    /**
     * @brief getFrame Returns a particular frame from the Sprite.
     * @param index The index of the frame to return.
     * @return A frame of a sprite.
     */
    Sprite *getFrame(int index);

    /**
     * @brief getFrameCount Gets the number of frames in the Sprite.
     * @return Returns an int representing the number of frames in the sprite.
     */
    int getFrameCount() const;

private:
    // The array of all frames in the sprite model.
    QVector<Sprite *> frames;

    // Stack containing all actions that can be undone..
    std::stack<Sprite *> undoStack;

    // Stack containing all actions that can be redone.
    std::stack<Sprite *> redoStack;

    // The current sprite frame that is copied.
    Sprite *clipBoardData = nullptr;

    // The index of the current frame being displayed
    int frameIndex;
signals:
    // Adds the sprite to the frameMenu
    void updateFrameMenu();

    // Creates a blank drawing space
    void newDrawingFrame(int frameIndex);

    // Deletes the frame from the frame display widget
    void deleteFrameFromWidget();

    // Displays the given sprite.
    void displaySprite(Sprite *currentSprite);

    // Toggle method which toggles the delete button on or off.
    void disableDeleteButton(bool disable);
};

#endif // SPRITEMODEL_H
