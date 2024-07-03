package jaxb;

import generated.Nalazista;
import java.io.File;
import javax.xml.XMLConstants;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;
import javax.xml.bind.ValidationEvent;
import javax.xml.bind.ValidationEventHandler;
import javax.xml.bind.ValidationEventLocator;
import javax.xml.validation.Schema;
import javax.xml.validation.SchemaFactory;
import org.xml.sax.SAXException;

public class JAXB {

    public static void main(String[] args) throws JAXBException, SAXException {
        try {
            JAXBContext jaxbContext = JAXBContext.newInstance(Nalazista.class);

            File xsd = new File("src/main/resources/xsd/nalazista.xsd");
            File xml = new File("src/main/resources/xml/Nalazista.xml");

            SchemaFactory schemaFactory = SchemaFactory.newInstance(XMLConstants.W3C_XML_SCHEMA_NS_URI);
            Schema schema = schemaFactory.newSchema(xsd);

            Unmarshaller unmarshaller = jaxbContext.createUnmarshaller();
            unmarshaller.setSchema(schema);

            unmarshaller.setEventHandler(new ValidationEventHandler() {
                @Override
                public boolean handleEvent(ValidationEvent event) {
                    ValidationEventLocator locator = event.getLocator();
                    System.out.println("XML nije ispravan. Greska u XML datoteci u liniji " + locator.getLineNumber() +
                                       ", stupcu " + locator.getColumnNumber() + 
                                       ": " + event.getMessage());
                    return false; 
                }
            });

            unmarshaller.unmarshal(xml);
            System.out.println("XML file je ispravan.");
        } catch (JAXBException e) {
            System.out.println("Greška XML");
        }
    }
}
