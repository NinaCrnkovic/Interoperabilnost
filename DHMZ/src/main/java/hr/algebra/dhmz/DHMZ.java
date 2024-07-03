package hr.algebra.dhmz;

import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.xml.stream.XMLStreamException;
import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

/**
 *
 * @author s_vre
 */
public class DHMZ {

    public static void main(String[] args) throws IOException, XMLStreamException {
        System.out.println("Pokretanje servera...");

        try {
            WebServer server = new WebServer(8086);

            XmlRpcServer xmlServer = server.getXmlRpcServer();
            PropertyHandlerMapping phm = new PropertyHandlerMapping();
            phm.addHandler("XML-RPC-DHMZ", XML.class);
            xmlServer.setHandlerMapping(phm);

            XmlRpcServerConfigImpl config = (XmlRpcServerConfigImpl) xmlServer.getConfig();
            config.setEnabledForExtensions(true);
            config.setContentLengthOptional(false);

            try {
                server.start();
            } catch (IOException ex) {
                Logger.getLogger(DHMZ.class.getName()).log(Level.SEVERE, null, ex);
            }
            System.out.println("Server pokrenut...");

        } catch (XmlRpcException ex) {
            Logger.getLogger(DHMZ.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
