import {
    Alert,
    Button,
    Dimensions,
    Image, Modal,
    ScrollView,
    StyleSheet,
    Text,
    TextInput,
    TouchableOpacity,
    View,
} from 'react-native';
import React, {useEffect, useRef, useState} from 'react';
import Icon from 'react-native-vector-icons/FontAwesome';
import store from '../store';
import {useRoute} from '@react-navigation/native';
import {COLORS} from '../Colors';
import {LogBox} from 'react-native';
import axios from "axios";
import Gallery from "react-native-image-gallery";
import Comment from "../Common/CommentsComponents/Comment";
import {storeData} from "../StorageHelper";
import CommentFillable from "../Common/CommentsComponents/CommentFillable";
import jwtDecode from "jwt-decode";
import StarRank from "../Common/CommentsComponents/StarRank";

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

export default function TouristSpot({navigation}) {
    const childRef = useRef(null);

    const [touristSpot, setTouristSpot] = useState([]);
    const [counter, setCounter] = useState(0);
    const route = useRoute();
    const url = route.params.spotUrl + "/true";
    const [refreshing, setRefreshing] = React.useState(false);
    const [modalVisible, setModalVisible] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            try {
                axios.post(url, {}).then((result) => {
                    setTouristSpot(result.data.Data)
                });
            } catch (error) {
                console.log('error', error);
            }
        };
        fetchData();

    }, [touristSpot.Name, url]);

    const onRefresh = React.useCallback(async () => {
        setRefreshing(true);
        setTimeout(() => {
            setRefreshing(false);
        }, 1000);
    }, []);

    function mapImagesToImageList(images) {
        return images.map(image => ({
            source: {uri: image.Photo}
        }));
    }

    return (
        <View style={styles.main}>
            <ScrollView>
                <View style={styles.container}>
                    <View style={styles.column}>
                        <Text style={styles.textTitle}>{touristSpot.Name}</Text>
                        {touristSpot.Score!==undefined?<StarRank score={touristSpot.Score}></StarRank>:null}
                    </View>
                    <View style={[styles.imageContainer, styles.elevation]}>
                        {touristSpot.Images == undefined ?
                            <Image style={styles.image}
                                   source={{uri: "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/2048px-No_image_available.svg.png"}}/>
                            :
                            <Gallery
                                style={styles.image}
                                images={mapImagesToImageList(touristSpot.Images)}
                            />}
                    </View>

                    <View style={styles.descContainer}>
                        <Text style={styles.textDescTitle}>Opis</Text>
                        <Text style={styles.textDesc}>{touristSpot.Description}</Text>
                        <Text style={styles.textDesc}>{touristSpot.Article}</Text>
                    </View>
                    <Text style={styles.textTitle}>Komentarze:</Text>
                    {store.getState().isLoggedIn ? (
                        <View style={styles.button}>
                            <TouchableOpacity disabled={!store.getState().isLoggedIn}
                                onPress={() => {
                                    setModalVisible(true)
                                }}>
                                <View style={styles.rowButton}>
                                    <Text style={styles.buttonText}>Dodaj Komentarz</Text>
                                </View>
                            </TouchableOpacity>
                        </View>
                    ) : null}
                </View>
                {touristSpot.Images == undefined ? null :
                    <Comment ref={childRef} commentUrl={"Comment/AllComentsForSpot/" + touristSpot.Id}></Comment>}
                <Modal
                    animationType="slide"
                    transparent={true}
                    visible={modalVisible}
                    onRequestClose={() => {
                        setModalVisible(!modalVisible);
                    }}>
                    <CommentFillable  touristSpottId={touristSpot.Id} CloseModal={() => {
                        childRef.current?.startFunction();
                        setModalVisible(false);
                    }}></CommentFillable>
                </Modal>
            </ScrollView>

        </View>

    );
}

const styles = StyleSheet.create({
    main: {
        backgroundColor: COLORS.second,
    },
    centeredView: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        marginTop: 22,
    },
    modalView: {
        margin: 20,
        backgroundColor: 'white',
        borderRadius: 20,
        padding: 35,
        alignItems: 'center',
        shadowColor: '#000',
        shadowOffset: {
            width: 0,
            height: 2,
        },
        shadowOpacity: 0.25,
        shadowRadius: 4,
        elevation: 5,
    },
    container: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'white',
        marginTop: 10,
    },
    column: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        alignContent: 'center',
    },
    row: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        alignContent: 'center',
        marginTop: 20,
        margin: 10,
    },
    counterText: {
        textAlign: 'center',
        fontSize: 22,
        color: COLORS.second,
        fontWeight: 800,
        marginLeft: 20,
        marginRight: 20,
    },
    icon: {
        margin: 0,
        fontSize: 30,
        color: COLORS.second,
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
    textTitle: {
        fontSize: 24,
        color: COLORS.second,
        marginTop: 20,
    },
    textDesc: {
        fontSize: 16,
        color: 'black',
    },
    textDescTitle: {
        fontSize: 20,
        color: COLORS.second,
        marginBottom: 10,
    },
    descContainer: {
        borderBottomWidth: 1,
        borderTopWidth: 1,
        borderColor: COLORS.second,
        borderRadius: 5,
        padding: 10,
        width: 350,
        marginTop: 10,
    },
    textPrice: {
        marginBottom: 20,
        fontWeight: 800,
        textAlign: 'center',
        fontSize: 22,
        color: COLORS.second,
    },
    imageContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        width: '100%',
        height: imageHeight,
    },
    elevation: {
        shadowColor: '#ff0000',
        elevation: 20,
    },
    image: {
        width: '100%',
        height: imageHeight,
        borderRadius: 5,
        borderColor: COLORS.main,
        borderWidth: 1,
    },
    button: {
        justifyContent: 'center',
        marginTop: 40,
        width: 250,
        height: 40,
        borderRadius: 20,
        paddingEnd:20,
        marginEnd:20,
        backgroundColor: COLORS.second,
    },
    buttonText: {
        textAlign: 'center',
        color: 'white',
        fontSize: 18,
        marginLeft: 20,
        fontWeight: 800,
    },
    buttonIcon: {
        color: COLORS.second,
        fontSize: 20,
    },
    rowButton: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
    },
});
