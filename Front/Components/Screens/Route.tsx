import React, {FC, useEffect, useState} from 'react';
import {
    Dimensions,
    Image, Modal,
    RefreshControl,
    ScrollView,
    StyleSheet,
    Text,
    TextInput,
    TouchableOpacity,
    View
} from 'react-native';
import AntDesignIcon from 'react-native-vector-icons/EvilIcons';
import {COLORS} from '../Colors';
import store from '../store';
import {checkIfLogged, checkIfLoggedAndLogout, getUserName, LogOut} from '../Utilities';
import RNFetchBlob from 'rn-fetch-blob';
import NetInfo, {refresh} from '@react-native-community/netinfo';
import CheckBox from '@react-native-community/checkbox';
import {API_URL, NOINTERNET, ROUTE, SERVER_ERROR} from "../actions";
import {showMessage} from 'react-native-flash-message';
import {Button} from 'react-native-elements';
import axios from "axios";
import {JwtDecodedResult, RouteInt} from "../Common/Interfaces";
import jwtDecode from "jwt-decode";
import {GestureHandlerRootView, Swipeable} from "react-native-gesture-handler";
import SwipableElement from "../Common/SwipableElement";
import ModalSpotsLists from "../Common/ModalSpotsLists";
import DragableListOfSpots from "../Common/DragableListOfSpots";
import {RecyclerListView} from "recyclerlistview";

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

const Route: FC<{ navigation: any }> = ({navigation}) => {
    const [Routes, setRoutes] = useState<RouteInt[]>([]);
    const [OpenedRoute, setOpenedRoute] = useState<RouteInt>({Id: 0, RouteName: ""});

    const [refreshing, setRefreshing] = useState<boolean>(false);
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));
    const [AddingRouteVisible, setAddingRouteVisible] = useState<boolean>(false);
    const [NewRouteName, setNewRouteName] = useState<string>();
    const [ModalSpotsVisible, setModalSpotsVisible] = useState<boolean>(false);

    const onRefresh = React.useCallback(async () => {
        setRefreshing(true);
        getRoutes()
        setTimeout(() => {
            setRefreshing(false);
        }, 1000);
    }, []);

    async function getRoutes() {
        axios.get(API_URL + ROUTE + "getUserRoutes/" + Token.jti, {headers: {Authorization: store.getState().token}}).then((response) => {
            console.log(response.data.Data)
            setRoutes(response.data.Data)
        }).catch((error) => {
        })
    }

    useEffect(() => {
        getRoutes();
    }, [navigation]);

    function OpenRoute() {

    }

    function DeleteRoute(item: RouteInt) {
        axios.get(API_URL + ROUTE + "RemoveUserRoute/" + item.Id, {headers: {Authorization: store.getState().token}}).then(response => {
            if (response.data.Status === 0) {
                onRefresh()
            }
        }).catch(error => {
        })
    }

    function AddRoute() {
        axios.post(API_URL + ROUTE + "AddUserRoute", {Id: 0, UserId: Token.jti, RouteName: NewRouteName}, {headers: {Authorization: store.getState().token}}).then((response) => {
            if (response.data.Status === 0) {
                showMessage({
                    message: 'Dodano nową ścieżkę.',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
                onRefresh()
            } else {
                showMessage({
                    message: 'Błąd: ' + response.data.Message + '.',
                    type: 'danger',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            }

        }).catch(error => {

        })
    }

    function openModal(item: RouteInt) {
        setOpenedRoute(item);
        setModalSpotsVisible(true)
    }

    return (
        <View style={{flex: 1}}>
            <View style={{width: imageWidth - 25 + 10, borderWidth: 2, borderRadius: 20, padding: 25, margin: 10}}>
                <Text style={{fontSize: 20, margin: 20, marginLeft: -(10), textAlign: "center", color: COLORS.second}}>Dodaj
                    nową trasę</Text>
                <View style={styles.row}>
                    <TextInput onBlur={() => setNewRouteName(NewRouteName)}
                               value={NewRouteName} onChangeText={(text) => setNewRouteName(text)}
                               placeholder={"RouteName"}
                               style={{flex: 3, padding: 20, borderWidth: 2, borderColor: COLORS.second, borderRadius: 20}}></TextInput>
                    <TouchableOpacity onPress={() => {
                        AddRoute()
                    }}
                                      style={{flex: 1, borderRadius: 20, margin: 10, borderColor: COLORS.second, borderWidth: 2, padding: 20}}><Text>Dodaj</Text></TouchableOpacity>
                </View>
            </View>
            {Routes.length === 0 ?
                <View style={{display: "flex", justifyContent: "center"}}>
                    <View style={{padding: 20, margin: 20, alignSelf: "center"}}>
                        <Text style={styles.emptyText}>Brak tras.</Text>
                    </View>
                </View>
                :
                <View style={{flex: 1}}>
                    <ScrollView
                        refreshControl={
                            <RefreshControl refreshing={refreshing} onRefresh={onRefresh}/>
                        }>
                        {Routes.map((item, i) => {
                            return (
                                <View key={i}>
                                    <SwipableElement
                                        OnClick={() => openModal(item)
                                        }
                                        LeftMessage={"Otwórz"}
                                        RightMessage={"Usuń"}
                                        CloseModal={() => {
                                        }} SwipeFromLeftAction={() => OpenRoute()}
                                        SwipeFromRightAction={() => DeleteRoute(item)}
                                        SwipeableItem={{Id: item.Id, ItemName: item.RouteName}}></SwipableElement>

                                </View>

                            );
                        })}
                        <Modal
                            animationType="slide"
                            transparent={true}
                            visible={ModalSpotsVisible}
                            onRequestClose={() => {
                                setModalSpotsVisible(!ModalSpotsVisible);
                            }}>
                                <DragableListOfSpots  Route={OpenedRoute} CloseModal={() =>
                                    setModalSpotsVisible(false)
                                }></DragableListOfSpots>

                        </Modal>
                    </ScrollView>
                </View>
            }
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
    modalView: {
        flex: 1,
        width: dimensions.width,
        justifyContent: 'flex-end',
        backgroundColor: 'transparent',
        borderRadius: 5,
        shadowColor: '#000',
        shadowOffset: {
            width: 0,
            height: 2,
        },
        shadowOpacity: 0.25,
        shadowRadius: 4,
        elevation: 5,
    },
    title: {
        fontSize: 14,
        fontWeight: '500',
        color: COLORS.second,
        marginLeft: 10,
        marginBottom: 10,
        textAlign: 'left',
    },
    sumText: {
        color: COLORS.second,
        fontSize: 18,
        fontWeight: '800',
        marginLeft: 10,
    },
    totalText: {
        color: COLORS.second,
        fontSize: 26,
        fontWeight: '800',
    },
    priceText: {
        color: COLORS.second,
        fontSize: 18,
        fontWeight: '800',
        marginBottom: 15,
    },
    rightBox: {
        alignItems: 'flex-end',
        marginLeft: 100,
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
    row: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        alignContent: 'center'
        // marginTop: 20,
        // margin: 10,
    },
    border: {
        margin: 10,
    },
    column: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        marginLeft: 10,
        // maxHeight: 60,
    },
    innerColumn: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'flex-start',
        alignContent: 'flex-start',
    },
    columnWide: {
        justifyContent: 'center',
        display: 'flex',
        alignItems: 'center',
        alignContent: 'center',
        flex: 1,
        // borderWidth: 1,
        height: imageHeight / 2.5,
    },
    counter: {
        margin: 0,
    },
    counterText: {
        textAlign: 'center',
        fontSize: 20,
        color: 'white',
        fontWeight: '800',
        marginBottom: 10,
        marginTop: 5,
    },
    counterContainer: {
        borderRadius: 20,
        paddingTop: 10,
        paddingBottom: 10,
        backgroundColor: COLORS.second,
        elevation: 10,
    },
    icon: {
        margin: 0,
        fontSize: 24,
        color: 'white',
    },
    imageContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        width: imageWidth / 4,
        height: imageHeight / 2.25,
        borderRadius: 50,
        elevation: 20,
    },
    image: {
        width: imageWidth / 4,
        height: imageHeight / 2.25,
        margin: 0,
        padding: 0,
        borderRadius: 50,
    },
    button: {
        justifyContent: 'center',
        marginTop: 20,
        width: 250,
        height: 40,
        borderRadius: 20,
        backgroundColor: COLORS.second,
    },
    buttonCode: {
        borderRadius: 20,
        backgroundColor: COLORS.second,
    },
    buttonText: {
        textAlign: 'center',
        color: 'white',
        fontSize: 18,
        fontWeight: '800',
    },
    marginBox: {
        marginTop: 20,
        marginBottom: 20,
    },
    input: {
        color: 'black',
        marginLeft: 10,
    },
    inputMargin: {
        marginBottom: 20,
    },
    infoText: {
        textAlign: 'center',
        color: COLORS.second,
        fontSize: 18,
    },
    emptyText: {
        textAlign: 'center',
        color: "gray",
        fontSize: 20,
    },
});

export default Route;
