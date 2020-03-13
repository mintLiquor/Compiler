#include "mainwindow.h"
#include"lexical.h"
#include <QApplication>
#include <QTextCodec>
#include<QSplashScreen>
#include<QDateTime>
int main(int argc, char *argv[])
{
   QApplication a(argc, argv);
   QTextCodec *codec = QTextCodec::codecForName("UTF-8");//情况2
   QTextCodec::setCodecForLocale(codec);
   QPixmap pixmap(":/resource/tongji1.PNG");
   QSplashScreen splash(pixmap);
   for(qint64 i=6666666;i>0;i--)
       splash.show();
    MainWindow w;
    w.setWindowIcon(QIcon(":/resource/tongji.ico"));
    w.setWindowTitle("语法分析器CopyRight@同济大学");
    w.show();
    splash.finish(&w);
    return a.exec();
}
