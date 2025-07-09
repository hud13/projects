/**
 * Declaration of the PlaybackWindow class.
 * Plays an animation of the current sprite using the current preview framerate.
 * The window size is fixed, and the sprite size is accurate to it's pixel size.
 *
 * @author Matthew Shaw
 * @date March 31, 2025
 * @reviewer Cheuk Yin Lau
 */

#include "playbackwindow.h"

PlaybackWindow::PlaybackWindow(const QVector<Sprite*>& frames, int fps, QWidget *parent)
    : QWidget(parent), frames(frames), currentIndex(0)
{
    // window configurations
    setFixedSize(400, 400);
    setAutoFillBackground(true);
    if (!frames.isEmpty())
    {
        QImage img = frames[0]->getImage();
        currentFrame = img;
    }

    // timer to control FPS
    timer = new QTimer(this);
    connect(timer,
            &QTimer::timeout,
            this,
            &PlaybackWindow::updateFrame);
    timer->start(1000 / fps);
}

void PlaybackWindow::updateFrame()
{
    if (frames.isEmpty()) return;

    currentFrame = frames[currentIndex]->getImage();
    update();  // triggers paintEvent
    currentIndex = (currentIndex + 1) % frames.size();
}


void PlaybackWindow::paintEvent(QPaintEvent *event)
{
    QPainter painter(this);
    painter.fillRect(rect(), Qt::white);  // fill background white

    // draw image at the center
    if (!currentFrame.isNull())
    {
        int x = (width() - currentFrame.width()) / 2;
        int y = (height() - currentFrame.height()) / 2;
        painter.drawImage(x, y, currentFrame);
    }

}
