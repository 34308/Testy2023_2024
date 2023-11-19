import React, {FC, useEffect, useState} from 'react';
import {
    Dimensions, FlatList, FlatListProps, Modal,
    Image,
    StyleSheet,
    Text,
    TextInput,
    TouchableOpacity,
    View, RefreshControl
} from 'react-native';
import {COLORS} from '../Colors';
import store from '../store';
import CheckBox from '@react-native-community/checkbox';
import {API_URL, NOINTERNET, ROUTE, SERVER_ERROR, TOURIST_SPOT} from "../actions";
import {showMessage} from 'react-native-flash-message';
import {Button} from 'react-native-elements';
import axios from "axios";
import {JwtDecodedResult, RouteInt} from "../Common/Interfaces";
import jwtDecode from "jwt-decode";
import StarRank from "../Common/CommentsComponents/StarRank";
import ModalSpotsLists from "../Common/ModalSpotsLists";
import TouristSpot from "./TouristSpot";
import navigation from "../Navigation";
import route from "./Route";

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

interface Address {
    Street: string,
    City: string,
    PostalCode: string,
    Country: string,
    Number: number,
    TouristSpotId: number,
}

interface ImageInterface {
    Id: number,
    TouristSpotId: number,
    Photo: string,
}
import {CommonActions, useNavigation} from '@react-navigation/native';

interface TouristSpotInterface {
    Name: string,
    Description: string,
    Score: number,
    Id: number,
    IsVisited: boolean,
    Address: Address
    MainPhoto: ImageInterface
}

function Visited(){
    const [refreshing, setRefreshing] = useState<boolean>(false);
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));
    const [VisitedTouristSpots, setVisitedTouristSpots] = useState<TouristSpotInterface[]>([]);
    const url = API_URL + TOURIST_SPOT+"TouristSpotsForCity/"
    const [modalVisible,setModalVisible]=useState<boolean>(false);
    const navigation = useNavigation();

    const onRefresh = React.useCallback(async () => {
        setRefreshing(true);
        getAllVisitedPlacesForUser();
        setTimeout(() => {
            setRefreshing(false);
        }, 1000);
    }, []);

    function getAllVisitedPlacesForUser() {
        try{
            if (store.getState().isLoggedIn) {
                axios.get(API_URL + TOURIST_SPOT + "getVisitedSpotsForUser/" + Token.jti, {headers: {Authorization: store.getState().token}})
                    .then(result => {
                        setVisitedTouristSpots(result.data.Data);
                    })
            } else {
                showMessage({
                    message: 'Użytkownik nie zalogowany',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            }
        }catch (error){
            showMessage({
                message: 'Błąd: ' + error,
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });
        }

    }

    useEffect(() => {
        const unsubscribe = navigation.addListener('focus', () => {
            getAllVisitedPlacesForUser();
        });
       return unsubscribe;
    }, [navigation]);

    function goToVisitedTouristSpot(param: string) {
        navigation.dispatch(CommonActions.navigate({
            name: 'TouristSpot',
            params: {spotUrl: param},
        }))
    }
    function VisitedSpotItem(item:TouristSpotInterface){
        return(
            <RefreshControl refreshing={refreshing}>
                <View key={item.Id}>
                    <View style={styles.row}>
                        <TouchableOpacity
                            onPress={() =>
                                goToVisitedTouristSpot(url + item.Address.City + "/" + item.Id)
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
                            <Text style={styles.textDesc}>{item.Description.substring(0, 90)}...</Text>
                            <View style={styles.row}>
                                <StarRank score={item.Score}></StarRank>
                            </View>

                        </View>

                        <View style={styles.columnPrice}>
                        </View>
                    </View>
                </View>
            </RefreshControl>

        );
    }

    return (
        <View style={{flex: 1}}>
            <FlatList
                data={VisitedTouristSpots}
                renderItem={({item}) =>
                    <VisitedSpotItem IsVisited={item.IsVisited}
                                     Name={item.Name}
                                     Address={item.Address}
                                     MainPhoto={item.MainPhoto}
                                     Score={item.Score}
                                     Description={item.Description}
                                     Id={item.Id}
                    />}
            ></FlatList>
            <Modal
                animationType="slide"
                transparent={true}
                visible={modalVisible}
                onRequestClose={() => {
                    setModalVisible(!modalVisible);
                }}>
                <TouristSpot navigation={navigation}></TouristSpot>
            </Modal>



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

export default Visited;
