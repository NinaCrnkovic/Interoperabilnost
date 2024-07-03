/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package hr.algebra.dhmz;

import hr.algebra.factory.ParserFactory;
import hr.algebra.factory.UrlConnectionFactory;
import hr.algebra.model.Temperatura;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Optional;
import javax.xml.stream.XMLEventReader;
import javax.xml.stream.XMLStreamConstants;
import javax.xml.stream.XMLStreamException;
import javax.xml.stream.events.Characters;
import javax.xml.stream.events.StartElement;
import javax.xml.stream.events.XMLEvent;

/**
 *
 * @author s_vre
 */
public class XML {
    private static final String URL = "https://vrijeme.hr/hrvatska_n.xml";
    
        public List<String> getTemperature(String name) throws IOException, XMLStreamException{
            List<Temperatura> temp = getGradovi();
            List<String> matchingTemperatures = new ArrayList<>();

            for (Temperatura temperatura : temp) {
                if(temperatura.getNaziv().contains(name)){
                    matchingTemperatures.add(temperatura.getNaziv() + ": " + temperatura.getTemperatura());
                }
            }

            if (!matchingTemperatures.isEmpty()) {
                return matchingTemperatures;
            } else {
                return Collections.singletonList("Nije nađeno");
            }
        }

    
    private List<Temperatura> getGradovi() throws IOException, XMLStreamException{
  
            List<Temperatura> temperature = new ArrayList<>();
            HttpURLConnection con = UrlConnectionFactory.getHttpUrlConnection(URL);
        try (InputStream is = con.getInputStream();){    
            
            XMLEventReader reader = ParserFactory.createStaxParser(is);
            Optional<Tag> tag = Optional.empty();
            Temperatura temperatura = null;
            StartElement startElement = null;
            
            while (reader.hasNext()) {
                XMLEvent event = reader.nextEvent();
                switch (event.getEventType()){
                    case XMLStreamConstants.START_ELEMENT ->{
                        startElement = event.asStartElement();
                        String qName = startElement.getName().getLocalPart();
                        tag = Tag.from(qName);
                        if (tag.isPresent() && tag.get().equals(Tag.NAZIV)) {
                            temperatura = new Temperatura();
                            temperature.add(temperatura);
                        }
                    }
                    case XMLStreamConstants.CHARACTERS ->{
                        if (tag.isPresent() && temperatura != null) {
                            Characters characters = event.asCharacters();
                            String data = characters.getData().trim();
                            switch (tag.get()) {
                                case NAZIV:
                                    if (!data.isEmpty()) {
                                        temperatura.setNaziv(data);
                                    }
                                    break;
                                case TEMPERATURA:
                                    if (!data.isEmpty()) {
                                        temperatura.setTemperatura(data);
                                    }
                                    break;
                                case VRIJEME:
                                    if (!data.isEmpty()) {
                                        temperatura.setVrijeme(data);
                                    }
                                    break;                   
                            }
                        }
                    }                
                }                    
            }
        }
        return temperature;
    }
   
    
    private enum Tag {
        NAZIV("GradIme"),
        TEMPERATURA("Temp"),
        VRIJEME("Vrijeme");

        private final String name;

        private Tag(String name) {
            this.name = name;
        }

        public static Optional<Tag> from(String name) {
            for (Tag value : values()) {
                if (value.name.equals(name)) {
                    return Optional.of(value);
                }
            }
            return Optional.empty();
        }
    }
}
