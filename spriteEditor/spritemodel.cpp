/**
 * Handles spritie project state. Manages frames, clipboard actions, and undo/redo history. Handles save / load from JSON.
 *
 * @author John Gibb
 * @date March 31, 2025
 * @reviewer Matthew Shaw
 */

#include "spritemodel.h"

SpriteModel::SpriteModel(int defaultSize, QObject *parent)
    : QObject(parent)
{
    Sprite *defaultSprite = new Sprite(defaultSize);
    frames.append(defaultSprite);
    frameIndex = 0;
}

void SpriteModel::loadProject(const QString &fileName)
{
    QFile file(fileName);
    if (!file.open(QIODevice::ReadOnly))
        return;

    QByteArray data = file.readAll();
    file.close();

    QJsonDocument doc = QJsonDocument::fromJson(data);
    QJsonObject root = doc.object();

    frames.clear();

    if (!root.contains("frames") || !root["frames"].isArray())
    {
        qWarning() << "Invalid file format: missing frames array";
        return;
    }

    QJsonArray framesArray = root["frames"].toArray();
    for (int i = 0; i < framesArray.size(); ++i)
    {
        QJsonObject spriteObj = framesArray[i].toObject();
        if (!spriteObj.contains("spriteSize") || !spriteObj.contains("pixels"))
        {
            qWarning() << "Frame" << i << "missing required fields";
            continue;
        }

        int size = spriteObj["spriteSize"].toInt();
        Sprite *sprite = new Sprite(size);
        QImage img(size, size, QImage::Format_ARGB32);

        QJsonArray rows = spriteObj["pixels"].toArray();
        for (int y = 0; y < size && y < rows.size(); ++y)
        {
            QJsonArray row = rows[y].toArray();
            for (int x = 0; x < size && x < row.size(); ++x)
            {
                QJsonObject pix = row[x].toObject();
                int r = pix["r"].toInt();
                int g = pix["g"].toInt();
                int b = pix["b"].toInt();
                int a = pix["a"].toInt();
                img.setPixelColor(x, y, QColor(r, g, b, a));
            }
        }

        sprite->setImage(img);
        frames.append(sprite);
    }

    if (frames.isEmpty())
    {

        // fallback if loading failed
        frames.append(new Sprite(2));
        frameIndex = 0;
    }
}

void SpriteModel::saveProject(const QString &fileName)
{
    QJsonObject root;
    QJsonArray framesArray;

    // iterate over each sprite frame
    for (int i = 0; i < frames.size(); ++i)
    {
        const Sprite *sprite = frames[i];
        QJsonObject spriteObj;
        QJsonArray rows;
        int size = sprite->getSpriteSize();

        // build the pixel array row by row
        for (int y = 0; y < size; ++y)
        {
            QJsonArray row;
            for (int x = 0; x < size; ++x)
            {
                QColor c = sprite->getPixel(x, y);
                QJsonObject pixelObj{{"r", c.red()},
                                     {"g", c.green()},
                                     {"b", c.blue()},
                                     {"a", c.alpha()}};
                row.append(pixelObj);
            }
            rows.append(row);
        }

        spriteObj["pixels"] = rows;
        spriteObj["spriteSize"] = size;
        framesArray.append(spriteObj);
    }

    root["frames"] = framesArray;

    // write the JSON document to file (use .ssp extension)
    QFile file(fileName);
    if (file.open(QIODevice::WriteOnly))
    {
        file.write(QJsonDocument(root).toJson(QJsonDocument::Indented));
        file.close();
    }
}

void SpriteModel::copy(int frame)
{
    if (frame < frames.size()) {
        clipBoardData = new Sprite(*frames[frame]);
    }
}

void SpriteModel::paste(int currentFrame)
{
    if (clipBoardData && currentFrame < frames.size())
    {
        Sprite *newFrame = new Sprite(*clipBoardData);
        frames.insert(currentFrame + 1, newFrame);
    }
}

void SpriteModel::addFrame(int currentFrame)
{
    frames.insert(currentFrame, new Sprite(frames.first()->getSpriteSize()));
    emit updateFrameMenu();
    emit displaySprite(frames.at(currentFrame));
}

void SpriteModel::deleteFrame(int framePos)
{
    if (framePos < frames.size() && frames.size() > 0)
    {
        Sprite *toDelete = frames[framePos];
        int spriteSize = frames[0]->getSpriteSize();
        frames.removeAt(framePos);
        delete toDelete;
        frameIndex = frames.count() - 1;
        if (frames.empty())
        {
            frames.append(new Sprite(spriteSize));
            frameIndex = 0;
            emit updateFrameMenu();
        }

        emit displaySprite(frames.last());
        emit disableDeleteButton(true);
    }
}

Sprite *SpriteModel::getFrame(int index)
{
    if (index >= 0 && index < frames.size())
        return frames[index];
    return nullptr;
}

int SpriteModel::getFrameCount() const
{
    return frames.size();
}
