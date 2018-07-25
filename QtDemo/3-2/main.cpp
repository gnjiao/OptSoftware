//窗体程序3大基类
//QMainWindow:   QMainWindow类提供一个有菜单栏,工具栏和一个转态栏的应用程序窗口
//    QWidget:   QWidget类是所有用户界面的基类,它从窗口系统接收鼠标,键盘和其它事件,并且在屏幕上绘制自己
//    QDialog:   QDialog类是对话框窗口的基类,对话框窗口主要用于短期任务以及和用户进行简要通信的顶级窗口

#include "dialog.h"
#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    Dialog w;
    w.show();

    return a.exec();
}
