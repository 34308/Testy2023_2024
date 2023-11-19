import {
  Text,
  TouchableOpacity,
  View,
  StyleSheet,
  ScrollView,
  Image,
  Dimensions,
  ImageBackground,
} from 'react-native';
import React, {useEffect, useState} from 'react';
import {COLORS} from '../Colors';
import {re} from '@babel/core/lib/vendor/import-meta-resolve';
import Icon from 'react-native-vector-icons/FontAwesome';
import NetInfo from '@react-native-community/netinfo';
import {API_URL, LOGOUT, NOINTERNET, SERVER_ERROR, SETPINS} from "../actions";
import axios from "axios";
import {getUserName} from "../Utilities";
import store from "../store";


const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

export default function Citys({navigation}) {
  const [touristSpots, setTouristSpots] = useState([]);
  const [numberOfTouristSpots, SetTouristSpotNumber] = useState(0);
  async function goToRestaurant(cityName) {
    navigation.navigate('TouristSpotsList', {
      restaurantUrl: url ,
      cityName: cityName,
    });
  }

  const url = API_URL + 'City/AllCitys';
  useEffect(() => {

    const unsubscribe = navigation.addListener('focus', () => {
      fetchData();
    });


    const fetchData = async () => {
      try {
        axios.get(url,).then((result)=>{
          SetTouristSpotNumber(result.data.Data.length)
          setTouristSpots(result.data.Data)
        }).catch(error=>{
          console.log(error)
        });

      } catch (error) {
        console.log('error', error);
      }
    };
    fetchData();
    return unsubscribe;
  }, [navigation]);

  return (
    <View style={styles.container}>
      <ScrollView>
        <View style={styles.restaurantsNumberBox}>
          <Text style={styles.restaurantsNumberText}>
            sprawdź {numberOfTouristSpots} miejsc.
          </Text>
        </View>
        {touristSpots.map((item, i) => {
          return (
            <TouchableOpacity key={i} onPress={() => goToRestaurant(item.Name)}>
              <View style={styles.box}>
                <View style={[styles.card, styles.elevation]}>
                  <View style={styles.backgroundContainer}>
                    <ImageBackground
                      source={{uri: item.BackgroundPhoto}}
                      style={styles.image}>
                      <View style={styles.logoContainer}>
                        <Image
                          style={styles.logo}
                          source={{uri: item.Crest}}
                        />
                      </View>
                    </ImageBackground>
                  </View>
                  <View style={styles.descContainer}>

                    <View>
                      <Text style={styles.textTitle}>{item.Name}</Text>
                      <View style={styles.row}>
                        <Text style={styles.textDesc}>{item.County}</Text>
                        <Text style={styles.textDesc}>{item.Commune}</Text>
                        <Text style={styles.textDesc}>{item.Voivodeship}</Text>
                      </View>
                    </View>
                  </View>
                </View>
              </View>
            </TouchableOpacity>
          );
        })}
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    display: 'flex',
    flexDirection: 'column',
    backgroundColor: 'white',
  },
  box: {
    justifyContent: 'center',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    alignContent: 'center',
  },
  backgroundContainer: {
    justifyContent: 'center',
    display: 'flex',
    flexDirection: 'column',
    flexWrap: 'wrap',
    alignItems: 'center',
    alignContent: 'center',
    width: '100%',
    borderRadius: 5,
  },
  card: {
    backgroundColor: 'white',
    borderRadius: 5,
    padding: 0,
    // paddingVertical: 45,
    // paddingHorizontal: 25,
    width: 350,
    marginVertical: 20,
  },
  elevation: {
    elevation: 5,
    shadowColor: '#52006A',
  },
  image: {
    width: '100%',
    height: imageHeight / 2,
  },
  logo: {
    width: imageWidth / 4,
    height: imageHeight / 3,
    resizeMode: 'contain',
    borderRadius: 60,
    borderWidth: 1,
    backgroundColor: COLORS.main,
  },
  logoContainer: {
    flex: 1,
    justifyContent: 'center',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    alignContent: 'center',
  },
  descContainer: {
    margin: 15,
  },
  row: {
    justifyContent: 'flex-start',
    display: 'flex',
    flexDirection: 'row',
    flexWrap: 'wrap',
    alignItems: 'center',
  },
  icon: {
    color: COLORS.second,
    fontSize: 16,
  },
  text: {
    color: COLORS.second,
    fontFamily: 'Ubuntu-Light',
    fontSize: 16,
    textAlign: 'center',
    marginLeft: 10,
  },
  textTitle: {
    color: COLORS.second,
    fontFamily: 'Ubuntu-Light',
    fontSize: 20,
    marginTop: 5,
    marginBottom: 5,
  },
  textDesc: {
    color: COLORS.fourth,
    fontFamily: 'Ubuntu-Light',
    fontSize: 14,
    textAlign: 'center',
    marginRight: 10,
  },
  restaurantsNumberBox: {
    justifyContent: 'flex-start',
    display: 'flex',
    flexDirection: 'column',
  },
  restaurantsNumberText: {
    color: COLORS.second,
    fontWeight: 800,
    fontSize: 20,
    marginTop: 20,
    marginBottom: 5,
    marginLeft: 30,
  },
});
