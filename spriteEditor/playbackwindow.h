/**
 * Declaration of the PlaybackWindow class.
 * Plays an animation of the current sprite using the current preview framerate.
 * The window size is fixed, and the sprite size is accurate to it's pixel size.
 *
 * @author Matthew Shaw
 * @date March 31, 2025
 * @reviewer Cheuk Yin Lau
 */

#ifndef PLAYBACKWINDOW_H
#define PLAYBACKWINDOW_H

#include <QWidget>
#include <QImage>
#include <QTimer>
#include "sprite.h"
#include <QPainter>

/**
 * The PlaybackWindow class dispalys a sequence of animation frames.
 * The sprite is in actual pixel size in a new window.
 *  A timer that follows the current framerate controls playback speed.
 */
class PlaybackWindow : public QWidget
{
    Q_OBJECT

public:

    /**
     * @brief PlaybackWindow Constructs a new PlaybackWindow that animates a list of frames.
     * @param frames Vector of sprite pointers representing animation frames.
     * @param fps Frames per second playback speed.
     * @param parent Optional parent QWidget.
     */
    explicit PlaybackWindow(const QVector<Sprite *> &frames, int fps = 10, QWidget *parent = nullptr);

protected:

    /**
     *  @brief paintEvent Overrides QWidget paintEvent to draw the current animation frame.
     *  @param Paint event triggered by the system.
     */
    void paintEvent(QPaintEvent *event) override;

private slots:

    /**
     *  @brief updateFrame Advances to the next animation frame and triggers a repaint.
     */
    void updateFrame();

private:

    QVector<Sprite *> frames;   /// Vector of frames to animate
    QImage frame;               /// Image used for rendering
    int currentIndex;           /// Index of the currently displayed frame
    QImage currentFrame;        /// The current frame being shown
    QTimer *timer;              /// Timer controlling the playback speed
};

#endif // PLAYBACKWINDOW_H
