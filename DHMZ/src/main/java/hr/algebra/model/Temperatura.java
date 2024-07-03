/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package hr.algebra.model;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlElement;

/**
 *
 * @author s_vre
 */
public class Temperatura {
    @XmlAttribute
    @XmlElement(name = "GradIme")
    private String naziv;
    
    @XmlElement(name = "Temp")
    private String temperatura;
    
    @XmlElement(name = "Vrijeme")
    private String vrijeme;

    public Temperatura() {
    }

    public Temperatura(String naziv, String temperatura, String vrijeme) {
        this.naziv = naziv;
        this.temperatura = temperatura;
        this.vrijeme = vrijeme;
    }

    public String getNaziv() {
        return naziv;
    }

    public void setNaziv(String naziv) {
        this.naziv = naziv;
    }

    public String getTemperatura() {
        return temperatura;
    }

    public void setTemperatura(String temperatura) {
        this.temperatura = temperatura;
    }

    public String getVrijeme() {
        return vrijeme;
    }

    public void setVrijeme(String vrijeme) {
        this.vrijeme = vrijeme;
    }
}
