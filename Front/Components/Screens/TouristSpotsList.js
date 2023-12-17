import {
    Dimensions,
    Image, Modal,
    ScrollView,
    StyleSheet,
    Text,
    TextInput,
    TouchableOpacity,
    View,
} from 'react-native';
import React, {useEffect, useMemo, useState} from 'react';
import {COLORS} from '../Colors';
import {useRoute} from '@react-navigation/native';
//import Icon from 'react-native-vector-icons/FontAwesome';
import Icon from 'react-native-vector-icons/Fontisto';
import store from '../store';
import CookieManager from '@react-native-cookies/cookies';
import {checkIfLogged, getUserName, LogOut} from '../Utilities';
import {showMessage} from 'react-native-flash-message';
import NetInfo from '@react-native-community/netinfo';
import {API_URL, DELETEPINS, LOGIN, LOGOUT, NOINTERNET, SERVER_ERROR, SETPINS, TOURIST_SPOT} from "../actions";
import axios from "axios";
import Searchbar from "../Common/Searchbar";
import {useDispatch} from "react-redux";
import CheckBox from "@react-native-community/checkbox";
import {red} from "react-native-reanimated/lib/types/lib";
import CustomCheckBox from "../Common/CustomCheckBox";
import jwtDecode from "jwt-decode";
import TouristSpot from "./TouristSpot";
import PressableStar from "../Common/CommentsComponents/PressableStar";
import StarRank from "../Common/CommentsComponents/StarRank";
import CommentFillable from "../Common/CommentsComponents/CommentFillable";
import ModalSpotsLists from "../Common/ModalSpotsLists";


const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

export default function TouristSpotsList({navigation}) {
    const [touristSpots, setTouristSpots] = useState([]);
    const [ftouristSpots, setfTouristSpots] = useState([]);
    const route = useRoute();
    const dispatch = useDispatch();
    const [modalVisible, setModalVisible] = useState(false);
    const [phrase, setPhrase] = useState('');
    const url = API_URL + TOURIST_SPOT + 'TouristSpotsForCityBasic/' + route.params.cityName;
    const [currentPins, setCurrentPins] = useState([])
    const [ChoosenTouristSpot, setChoosenTouristSpot] = useState({})


    const TouristSpoturl =
        API_URL + TOURIST_SPOT + 'TouristSpotsForCity/' + route.params.cityName + "/";

    async function addPlaceToNavigation(place) {
        console.log('check123')
        const coordinates = {
            id: place.Id,
            region: {
                latitude: place.Latitude,
                longitude: place.Longitude,
                latitudeDelta: 0.01,
                longitudeDelta: 0.01,
            }
        }
        if (currentPins.length > 0 && currentPins!==undefined) {

            const idExists = currentPins.some(item => item.id === coordinates.id);

            if (!idExists) {
                currentPins[currentPins.length] = coordinates;
                console.log(currentPins)
                dispatch({type: SETPINS, payload: JSON.stringify(currentPins)});
                showMessage({
                    message: 'dodano do nawigacji.',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            } else {
                //already in
                showMessage({
                    message: 'Juz dodano do nawigacji.',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            }
        } else {
            dispatch({type: SETPINS, payload: JSON.stringify([coordinates])});
            showMessage({
                message: 'dodano do nawigacji.',
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });

        }
        setCurrentPins(JSON.parse(await store.getState().Pin));
    }

    async function removeFromNavigation(place) {
        const coordinates = {
            id: place.Id,
            region: {
                latitude: place.Latitude,
                longitude: place.Longitude,
                latitudeDelta: 0.01,
                longitudeDelta: 0.01,
            }
        }
        if (currentPins.length > 0) {
            const idExists = currentPins.some(item => item.id === coordinates.id);
            if (!idExists) {
                showMessage({
                    message: 'Nie znaleziono w nawigacji',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            } else {
                //already in
                const newPins = currentPins.filter((item, i) => {
                    return item.id !== coordinates.id;
                });
                dispatch({type: SETPINS, payload: JSON.stringify(newPins)});
                showMessage({
                    message: 'Usunieto z nawigajci',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            }
        } else {
            showMessage({
                message: 'Nawigacja jest pusta.',
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });
        }
        setCurrentPins(JSON.parse(await store.getState().Pin));
    }
    const setVisited = useMemo(() => {
        getAllVisitedPlaces()
    }, [touristSpots]);

    useEffect(() => {
        console.log("TOKEN1:" + store.getState().token)
        // dispatch({type: DELETEPINS});
        const fetchData = () => {
            try {
                axios.post(url, {}).then((result) => {
                    setTouristSpots(result.data.Data)
                });
            } catch (error) {
                console.log('error', error);
            }
        };
        async function setPins() {
            if (await store.getState().Pin) {
                setCurrentPins(JSON.parse(await store.getState().Pin));
            }
        }
        fetchData();
        setPins();

    }, [url]);

    async function getAllVisitedPlaces() {
        if (store.getState().isLoggedIn) {
            axios.get(API_URL + 'TouristSpot/getVisitedSpotsId/' + jwtDecode(store.getState().token).jti, {headers: {Authorization: store.getState().token}})
                .then(result => {
                    const updatedTouristSpots = touristSpots.map(item => {

                        const isVisited = result.data.Data.some(ind => item.Id === ind.TouristSpotId);
                        return {...item, IsVisited: isVisited};
                    });
                    setfTouristSpots(updatedTouristSpots)
                })
        } else {
            setfTouristSpots(touristSpots)
        }
    }

    function IsInNavigation(place) {
        if(currentPins!==null){
            return currentPins.some(item => item.id === place.Id);
        }
        else{
            return false;
        }
    }
    function goToTouristSpot(spotUrl) {
        navigation.navigate('TouristSpot', {
            spotUrl: spotUrl,
        });
    }

    async function AddToVisited(Spotid) {

        if (await checkIfLogged()) {
            console.log(2)
            axios.get(API_URL + 'TouristSpot/AddVisitedSpot/' + Spotid + '/' + jwtDecode(store.getState().token).jti, {headers: {Authorization: store.getState().token}})
                .then(r => {
                    console.log(r.data)
                    showMessage({
                        message: 'Dodano do odwiedzonych.',
                        type: 'info',
                        backgroundColor: COLORS.second,
                        color: COLORS.main,
                    });
                }).catch(error => {
                showMessage({
                    message: 'Błąd podczas dodawania do odwiedzonych.',
                    type: 'info',
                    backgroundColor: COLORS.second,
                    color: COLORS.main,
                });
            })

        } else {
            LogOut(navigation, store.dispatch);
        }
    }

    async function RemoveFromVisited(Spotid) {
        if (await checkIfLogged()) {
            console.log(2)
            axios.get(API_URL + 'TouristSpot/RemoveVisitedSpot/' + Spotid + '/' + jwtDecode(store.getState().token).jti, {headers: {Authorization: store.getState().token}})
                .then(r => {
                    console.log(r.data)
                    showMessage({
                        message: 'usunieto z odwiedzonych.',
                        type: 'info',
                        backgroundColor: COLORS.second,
                        color: COLORS.main,
                    });
                }).catch(error => {
                console.log(4)
                showMessage({
                    message: 'Błąd podczas usuwania do odwiedzonych.',
                    type: 'info',
                    backgroundColor: COLORS.second,
                    color: COLORS.main,
                });
            })

        } else {
            LogOut(navigation, store.dispatch);
        }
    }

    function AddToSightseeingList(item) {

    }

    function closeModal() {
        setModalVisible(false)
    }

    return (

        <View style={{marginTop:20}}>
            <Searchbar  OnSearch={(value) => setPhrase(value)}></Searchbar>
            <View style={styles.container}>
                <ScrollView style={{flexDirection:'column',marginTop:20,maxHeight:dimensions.height-70}}>
                    {ftouristSpots
                        .filter(function (TouristSpot) {
                            return TouristSpot.Name.toLowerCase().includes(phrase.toLowerCase());
                        })
                        .map((item, i) => {
                            return (
                                <View key={i}>
                                    <View style={styles.row}>
                                        <TouchableOpacity
                                            onPress={() =>
                                                goToTouristSpot(TouristSpoturl + item.Id)
                                            }>
                                            <View style={styles.imageContainer}>
                                                <Image
                                                    style={styles.image}
                                                    source={{uri: item.MainPhoto.Photo}}
                                                />
                                            </View>
                                        </TouchableOpacity>
                                        <View style={styles.column}>
                                            <Text style={styles.textTitle}>{item.Name}</Text>
                                            <Text style={styles.textDesc}>{item.Description.substring(0,90)}...</Text>
                                            <View style={styles.row}>
                                                <StarRank score={item.Score}></StarRank>
                                            </View>
                                            <Text style={styles.line}/>
                                        </View>

                                        <View style={styles.columnPrice}>
                                            {store.getState().isLoggedIn ?
                                                <CustomCheckBox style={styles.icon}
                                                                initialState={item.IsVisited}
                                                                onUnchecked={() => {
                                                                    RemoveFromVisited(item.Id)
                                                                }}
                                                                onChecked={(i) => {
                                                                    AddToVisited(item.Id)
                                                                }}></CustomCheckBox> : null}
                                            <TouchableOpacity
                                                onPress={() => {
                                                    IsInNavigation(item) ? removeFromNavigation(item) : addPlaceToNavigation(item)
                                                }}>
                                                <View style={styles.plusContainer}>
                                                    <Icon name="navigate"
                                                          style={IsInNavigation(item) ? styles.iconPlus : styles.iconMinus}/>
                                                </View>
                                            </TouchableOpacity>
                                            {store.getState().isLoggedIn ?
                                                <TouchableOpacity style={styles.icon}
                                                                  initialState={item.IsVisited}
                                                                  onPress={() => {
                                                                      setChoosenTouristSpot(item);
                                                                      setModalVisible(!modalVisible);
                                                                  }}>
                                                    <Icon name="plus-a" style={styles.icon}/>
                                                </TouchableOpacity> : null}
                                        </View>
                                    </View>
                                </View>
                            );
                        })}

                </ScrollView>
                <Modal
                    animationType="slide"
                    transparent={true}
                    visible={modalVisible}
                    onRequestClose={() => {
                        setModalVisible(!modalVisible);
                    }}>

                    <ModalSpotsLists TouristSpotName={ChoosenTouristSpot.Name} TouristSpotId={ChoosenTouristSpot.Id}
                                     onModalClose={closeModal}></ModalSpotsLists>
                </Modal>
            </View>

        </View>

    );
}

const styles = StyleSheet.create({
    container: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'white',
        marginTop: 10,
    },
    modalView: {
        flex: 1,
        justifyContent: 'center',
        margin: 10,
        backgroundColor: 'transparent',
        borderRadius: 5,
        padding: 15,

        shadowColor: '#000',
        shadowOffset: {
            width: 0,
            height: 2,
        },
        shadowOpacity: 0.25,
        shadowRadius: 4,
        elevation: 5,
    },
    dishContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'white',
        margin: 0,
    },
    textSearchBar: {
        color: COLORS.second,
        fontFamily: 'Ubuntu-Light',
        fontSize: 16,
        textAlign: 'left',
    },
    textTitle: {
        fontSize: 16,
        color: COLORS.third,
        marginBottom: 10,
    },
    textDesc: {
        fontSize: 14,
        color: COLORS.fifth,
    },
    textPrice: {
        fontSize: 24,
        color: COLORS.fifth,
        fontWeight: 800,
        marginBottom: 10,
    },
    column: {
        width: 190,
        marginRight: 10,
    },
    columnPrice: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
    },
    row: {
        flex: 1,
        flexDirection: 'row',
        marginTop: 10,
        padding: 5,
        paddingBottom: 15,
        borderBottomWidth: 1,
        borderColor: COLORS.second,
    },
    plusContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        width: 35,
        borderRadius: 5,
        backgroundColor: COLORS.second,
        marginTop: 20,
    },
    searchBar: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'white',
        borderColor: COLORS.second,
        borderWidth: 1,
        borderRadius: 5,
        width: 380,
        height: 50,
        marginLeft: 15,
    },
    icon: {
        fontSize: 30,
        margin: 5,
        color: COLORS.second
    },
    iconPlus: {
        fontSize: 26,
        padding: 5,
        color: 'orange',
    },
    iconMinus: {
        fontSize: 26,
        padding: 5,
        color: 'white',
    },
    imageContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        width: imageWidth / 3.5,
        height: imageHeight / 2,
        borderWidth: 1,
        borderRadius: 5,
        borderColor: COLORS.mainOrange,
        backgroundColor: COLORS.secondOrange,
        marginRight: 15,
        marginLeft: 15,
    },
    image: {
        // flex: 1,
        // resizeMode: 'contain',
        // flex: 1,
        width: imageWidth / 3.5,
        height: imageHeight / 2,
        resizeMode: 'cover',
        borderRadius: 5,
        borderColor: COLORS.second,
        borderWidth: 1,
        margin: 20,
    },
});
