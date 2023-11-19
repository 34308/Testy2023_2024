import React, {FC, useCallback, useEffect, useState} from 'react';
import {View, StyleSheet, Dimensions, TouchableOpacity, Modal, Linking, Platform} from 'react-native';
import MapView, {Marker, Region} from 'react-native-maps';
import {ListItem} from 'react-native-elements';
import {Swipeable} from 'react-native-gesture-handler';
import Geolocation from 'react-native-geolocation-service';
import {checkIfLogged} from '../Utilities';
import {COLORS} from '../Colors';
import Icon from 'react-native-vector-icons/FontAwesome5';
import store from '../store';
import ModalRoutesList from '../Common/ModalRoutesList';
import {useFocusEffect} from "@react-navigation/native";
import PinsLocationList from "../Common/PinsLocationList";
import MapViewDirections from 'react-native-maps-directions';
import {GOOGLE_API_KEY} from "../actions";

const dimensions = Dimensions.get('window');
const height = dimensions.height;
const width = dimensions.width;

interface Pin {
    id:number;
    name:string;
    region: Region;
}

function Map(navigation: any) {
    const [mapReady, onMapLayout] = useState(false);
    const [region, setRegion] = useState<Region>({
        latitude: 50.0128,
        longitude: 20.9888,
        latitudeDelta: 0.01,
        longitudeDelta: 0.01,
    });
    const [Pins, setPins] = useState<Pin[]>([]);
    const [it, setIt] = useState(0);
    const [PinsListVisible, SetPinListVisible] = useState(false);
    const [RouteListVisible, SetRouteListVisible] = useState(false);
    const [UserLocation, setUserLocation] = useState<Region>();
    const [Logged, setLogged] = useState(false);
    const [Navigate, setNavigate] = useState(false);
    const [NavigateIndex, setNavigateIndex] = useState(0);

    const refreshComponent = useCallback(async () => {
        if (store.getState().Pin) {
            const pins = JSON.parse(await store.getState().Pin);
            if (pins == null) {
                setPins([]);
            } else {
                setPins(pins);
            }
        }
    }, []);
    useFocusEffect(
        useCallback(() => {
            refreshComponent();
            return () => {
                // Optional: Any cleanup logic goes here
            };
        }, [refreshComponent])
    );
    useEffect(() => {
        getAndFormatCurrentLocation();

        checkIfLogged().then(result => {
            setLogged((result))
        });
        console.log('LOGGED:' + Logged);

    }, [navigation]);

    const regionChangeHandler = async (e: Region) => {
        // setIt(it+1)
        // setRegion(Pins[it].region);
    };

    function GotoApp() {
        const origin = `${Pins[0].region.latitude},${Pins[0].region.longitude}`;
        const destination = `${Pins[Pins.length - 1].region.latitude},${Pins[Pins.length - 1].region.longitude}`;

        const waypointsString = Pins.slice(1, -1)
            .map((wp) => `${wp.region.latitude},${wp.region.longitude}`)
            .join('/');

        const scheme = Platform.select({ios: 'maps://', android: 'https://www.google.com/maps/dir/'});

        const url = Platform.select({
            ios: `${scheme}?saddr=${origin}&daddr=${destination}&dirflg=d&waypoints=${waypointsString}`,
            android: `${scheme}${origin}/${destination}/${waypointsString}/data=!4m2!4m1!3e2!5i2!3m1!2i`,
        });
        Linking.openURL(url as string);
    }

    function getAndFormatCurrentLocation() {
        if (true) {
            Geolocation.getCurrentPosition(
                (position) => {
                    setUserLocation({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                        longitudeDelta: 0.1,
                        latitudeDelta: 0.1,
                    });
                    setRegion({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                        longitudeDelta: 0.1,
                        latitudeDelta: 0.1,
                    });
                },
                (error) => {
                    // See error code charts below.
                    console.log(error.code, error.message);
                },
                {enableHighAccuracy: true, timeout: 15000, maximumAge: 10000}
            );
        }
    }
    function NextLocation(){
        let index=NavigateIndex;
        index++;
        if(index>=Pins.length){
            return;
        }

        setNavigateIndex(index);
    }
    function PrevLocation(){
        let index=NavigateIndex;
        index--;

        if(index<0){
            return;
        }
        setNavigateIndex(index);
    }
    async function getNewPins() {
        const pins = JSON.parse(await store.getState().Pin);
        setPins(pins);
        setRegion(pins[0].region);
    }

    return (
        <View style={styles.container}>
            <MapView
                showsUserLocation={true}
                showsMyLocationButton={true}
                onRegionChangeComplete={regionChangeHandler}
                zoomEnabled={true}
                region={region}
                style={styles.mapcontainer}
                onMapLoaded={() => {
                    onMapLayout(true);
                }}>
                {(Pins.length>0 && Navigate)? <MapViewDirections
                    origin={UserLocation}
                    destination={Pins[NavigateIndex].region}
                    apikey={GOOGLE_API_KEY}

                />:null}
                {mapReady
                    ? Pins.map((item, i) => {
                        return <Marker title={i+1+"."+item.name}  key={i} coordinate={item.region}/>;
                    })
                    : null}
            </MapView>

            <View style={styles.row}>
                <TouchableOpacity
                    onPress={() => {
                        GotoApp();
                    }}>
                    <Icon style={styles.icon} name="map-marked-alt"></Icon>
                </TouchableOpacity>
                {Logged ? (
                    <TouchableOpacity
                        onPress={() => {
                            getAndFormatCurrentLocation();
                            SetRouteListVisible(!RouteListVisible);
                        }}>
                        <Icon style={styles.icon} name="clipboard-list"></Icon>
                    </TouchableOpacity>
                ) : null}

                <TouchableOpacity
                    onPress={() => {
                        SetPinListVisible(!PinsListVisible);
                    }}>
                    <Icon style={styles.icon} name="list"></Icon>
                </TouchableOpacity>
                <TouchableOpacity
                    onPress={() => {
                        PrevLocation();
                    }}>
                    <Icon style={styles.icon} name="arrow-left"></Icon>
                </TouchableOpacity>
                <TouchableOpacity
                    onPress={() => {
                        setNavigate(!Navigate);
                    }}>
                    <Icon style={styles.icon} name="route"></Icon>
                </TouchableOpacity>
                <TouchableOpacity
                    onPress={() => {
                        NextLocation();
                    }}>
                    <Icon style={styles.icon} name="arrow-right"></Icon>
                </TouchableOpacity>
            </View>
            <Modal
                style={styles.bottomModal}
                animationType="slide"
                transparent={true}
                visible={PinsListVisible}
                onRequestClose={() => {
                    SetPinListVisible(!PinsListVisible);
                }}>
                <PinsLocationList OnDeletePin={getNewPins} pins={Pins}></PinsLocationList>
            </Modal>
            <Modal
                style={styles.bottomModal}
                animationType="slide"
                transparent={true}
                visible={RouteListVisible}
                onRequestClose={() => {
                    SetRouteListVisible(!RouteListVisible);
                }}>
                <ModalRoutesList UserLocation={UserLocation as Region} CloseModal={() => SetRouteListVisible(false)}
                ></ModalRoutesList>
            </Modal>
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        display: 'flex',
        flex: 1,
    },
    bottomModal: {
        width: dimensions.width,
        justifyContent: 'flex-end',
        margin: 0,
    },
    mapcontainer: {
        flex: 1,
        width: width,
        height: height,
    },
    row: {
        borderRadius: 10,
        justifyContent: 'flex-end',
        display: 'flex',
        flexDirection: 'row',
        alignContent: 'flex-start',
        marginTop: 0,
        margin: 10,
        padding: 15,
    },
    icon: {
        margin: 20,
        fontSize: 24,
        color: COLORS.second,
    },
});

export default Map;
