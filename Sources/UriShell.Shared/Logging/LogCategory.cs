using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace UriShell.Logging
{
    /// <summary>
    /// ������ ���� ��������� ������� ����.
    /// </summary>
    public enum LogCategory
    {
        /// <summary>
        /// ���������, ��������������� ��� ���������� �������.
        /// </summary>
        Debug,

        /// <summary>
        /// ��������� ��� ������ ����������, ����������� ������.
        /// </summary>
        Exception,

        /// <summary>
        /// ��������� �������������� ���������.
        /// </summary>
        Information,

        /// <summary>
        /// ��������� ��� ������ ���������, ��������� ����������� ��������.
        /// </summary>
        Warning
    }
}