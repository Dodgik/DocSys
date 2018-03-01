<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="text"/>
    <xsl:template match="/">
        <xsl:text>{\rtf1\ansi\ansicpg1251\deff0\deflang1049\deflangfe1049\deftab708{\fonttbl{\f0\fswiss\fprq2\fcharset204 Calibri;}{\f1\froman\fprq2\fcharset204{\*\fname Times New Roman;}Times New Roman CYR;}}</xsl:text>

        <xsl:text>\viewkind4\uc1</xsl:text>
        
        <xsl:text>\pard\sa200\sl276\slmult1\qc\lang1058\f0\fs28</xsl:text>
        <xsl:for-each select="StatisticsReport/Header/string">
            <xsl:text> </xsl:text>
            <xsl:value-of select="."/>
            <xsl:text>\par</xsl:text>
        </xsl:for-each>

        <xsl:text>\pard\sa200\sl276\slmult1</xsl:text>
        <xsl:for-each select="StatisticsReport/Content/string">
            <xsl:text> </xsl:text>
            <xsl:value-of select="."/>
            <xsl:text>\par</xsl:text>
        </xsl:for-each>
        
        <xsl:for-each select="StatisticsReport/Tables/TableForm">

            <xsl:text>\trowd\trgaph108\trleft-108\trrh678\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
            
            <xsl:for-each select="Columns/int">
                <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx</xsl:text>
                <xsl:value-of select="."/>
            </xsl:for-each>
        
            <xsl:text>\pard\intbl\nowidctlpar </xsl:text>
            <xsl:text>\qc\fs24</xsl:text>
        
            <xsl:for-each select="Rows/ArrayOfString">
                <xsl:text>\trowd\trgaph108\trleft-108\trrh404\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>

                <xsl:for-each select="../../Columns/int">
                    <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx</xsl:text>
                    <xsl:value-of select="."/>
                </xsl:for-each>
                <xsl:text>\pard\intbl\nowidctlpar </xsl:text>
                <xsl:for-each select="string">
                    <xsl:value-of select="."/>
                    <xsl:text>\cell </xsl:text>
                </xsl:for-each>

                <xsl:text>\row </xsl:text>
            </xsl:for-each>
            <xsl:text>\pard\sa200\sl276\slmult1\fs28</xsl:text>
            <xsl:text>\par</xsl:text>
        </xsl:for-each>
        
        <xsl:text>\par</xsl:text>
        <xsl:for-each select="StatisticsReport/Footer/string">
            <xsl:text> </xsl:text>
            <xsl:value-of select="."/>
            <xsl:text>\par</xsl:text>
        </xsl:for-each>

        <xsl:text>\'c2\'e8\'ea\'ee\'ed\'e0\'e2 ______________________ \par</xsl:text>

        <xsl:text>}</xsl:text>

    </xsl:template>
</xsl:stylesheet>