<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:template match="/Logs">
    <table class="DSDRLogs">
      <tr class="DSDRLogsHead">
        <td class="DSDRLogHead" width="200px">
          Timestamp
        </td>
        <td class="DSDRLogHead" width="200px">
          Step
        </td>
        <td class="DSDRLogHead" width="100px">
          State
        </td>
        <td class="DSDRLogHead" width="430px">
          Message
        </td>
        <td class="DSDRLogHead" width="10px">
        </td>
      </tr>
      <xsl:for-each select="Log">
        <tr class="DSDRLog0" >
          <xsl:attribute name="group">
            <xsl:value-of select="@Group"/>
          </xsl:attribute>
          <xsl:if test="@NewStepKind='true'">
            <xsl:attribute name="class">
              DSDRLog1
            </xsl:attribute>
          </xsl:if>
          <td>
            <xsl:value-of select="@Timestamp"/>
          </td>
          <td>
            <xsl:value-of select="@Step"/>
          </td>
          <td>
            <xsl:value-of select="@State"/>
          </td>
          <td>
            <xsl:value-of select="@Message"/>
          </td>
          <xsl:if test="@TogglesCollapse='true'">
            <td>
              <img src="images/arrow_up.png"  onclick="ToggleCollapse(this)" class="toggleCollapseImg" title="click collapse/open" style="cursor:pointer;" />
            </td>
          </xsl:if>
          <xsl:if test="@TogglesCollapse='false'">
            <td>
             
            </td>
          </xsl:if>

        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

</xsl:stylesheet>
