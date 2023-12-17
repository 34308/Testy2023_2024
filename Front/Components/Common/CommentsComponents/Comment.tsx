import React, {forwardRef, useEffect, useImperativeHandle, useState} from "react";
import axios from "axios";
import {API_URL, COMMENT, NOINTERNET, SERVER_ERROR} from "../../actions";
import {COLORS} from "../../Colors";
import {Image, Modal, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View} from "react-native";
import Icon from "react-native-vector-icons/FontAwesome";
import StarRank from "./StarRank";
import {Button} from "react-native-elements";
import {
    Menu,
    MenuOptions,
    MenuOption,
    MenuTrigger,
} from 'react-native-popup-menu';
import Animated from "react-native-reanimated";
import store from "../../store";
import {JwtDecodedResult} from "../Interfaces";
import jwtDecode from "jwt-decode";
import {showMessage} from "react-native-flash-message";
import CommentFillable from "./CommentFillable";
import CommentForCommentFillable from "./CommentForCommentFillable";
import CommentForComment from "./CommentForComment";
import FoldableButton from "./FoldableButton";


interface CommentProps {
    commentUrl: string;
}

export type ChildComponentHandles = {
    startFunction: () => void;
};

interface CommentData {
    Id: number;
    Avatar: string;
    Username: string;
    Score: number;
    Title: string;
    Description: string;
    TouristSpotId: number;
    CommentChildNumber: number;
}

const Comment = forwardRef<ChildComponentHandles, CommentProps>(({commentUrl}, ref) => {
    const [comment, setComment] = useState<CommentData[]>([]);
    const [editable, setEditable] = useState<boolean[]>([]);
    const [Token, setToken] = useState<JwtDecodedResult>();
    const [ModalVisible, setModalVisible] = useState<boolean>(false);
    const [ItemForModal, setItemForModal] = useState<CommentData>();
    const [foldable, setFoldable] = useState<boolean[]>([]);

    const url = API_URL + commentUrl;
    useImperativeHandle(ref, () => ({
        startFunction: () => {
            fetchData();
            // Other logic here
        },
    }));

    function createBooleanArray(objects: object[]): boolean[] {
        return new Array(objects.length).fill(false);
    }

    const fetchData = async () => {
        try {
            axios.get(url).then((result) => {
                setComment(result.data.Data);
                setEditable(createBooleanArray(result.data.Data))
                setFoldable(createBooleanArray(result.data.Data))

            });
        } catch (error) {
            console.log("error", error);
        }
    };
    useEffect(() => {
        if (store.getState().isLoggedIn) {
            setToken(jwtDecode(store.getState().token));
        }
        fetchData();
    }, [url]);

    function SaveEditedComment(item: CommentData,i:number) {
        try {

            axios.post(API_URL + COMMENT + "update/" + Token?.jti,
                {
                    "id": "" + item.Id,
                    "title": "" + item.Title,
                    "description": "" + item.Description,
                    "score": "" + item.Score,
                    "userId": 0,
                    "touristSpotId": 0,
                    "parentCommentId": 0
                }
                , {headers: {Authorization: store.getState().token}}).then((result) => {
                    if(result.data.Status==0){
                        showMessage({
                            message: 'zmieniono poprawnie.',
                            type: 'info',
                            backgroundColor: COLORS.second,
                            color: COLORS.main,
                        });
                        const temp = editable.slice(0);
                        temp[i] = false;
                        setEditable(temp)
                    }else{
                        if(result.data.Status==25){
                            showMessage({
                                message: 'znaleziono przekleństwo: '+ result.data.Data,
                                type: 'info',
                                backgroundColor: COLORS.second,
                                color: COLORS.main,
                            });
                        }

                    }
            });
        } catch (error) {
            console.log("error", error);
        }
    }

    function DeleteComment(item: CommentData) {
        axios.get(API_URL + COMMENT + "remove/" + Token?.jti + "/" + item.Id, {headers: {Authorization: store.getState().token}}).then((result) => {
            if (result.data.Status == 0) {
                fetchData();
                showMessage({
                    message: 'Usunięto komentarz.',
                    type: 'info',
                    backgroundColor: COLORS.second,
                    color: COLORS.main,
                });
            }
        }).catch(Error)
        {
            showMessage({
                message: 'Błąd: '+Error,
                type: 'danger',
                backgroundColor: COLORS.second,
                color: COLORS.main,
            });
        }
    }

    return (
        <View style={styles.main}>
            <View style={styles.container}>
                {comment.map((item, i) => {
                    return (
                        <View key={i}>
                            <View style={styles.row}>
                                <View style={{justifyContent: "flex-end", alignSelf: "flex-end"}}>
                                    {(Token?.sub == item.Username || Token?.role == 99) ? <Menu>
                                        <MenuTrigger text={"..."}/>
                                        <MenuOptions>
                                            <MenuOption onSelect={() => {
                                                const temp = editable.slice(0);
                                                temp[i] = true;
                                                setEditable(temp)
                                            }} text='Edytuj'/>
                                            <MenuOption onSelect={() => {
                                                DeleteComment(item);
                                            }}>
                                                <Text style={{color: 'red'}}>Usuń</Text>
                                            </MenuOption>
                                        </MenuOptions>
                                    </Menu> : null}
                                </View>
                                <View style={styles.row2}>
                                    <View style={styles.partRow1}>
                                        <Image
                                            style={{
                                                height: 40,
                                                width: 40,
                                                marginBottom: 10,
                                            }}
                                            source={{uri: item.Avatar}}
                                        ></Image>
                                        <Text style={styles.textLogin}>{item.Username}</Text>
                                    </View>
                                    <View style={styles.partRow2}>
                                        <StarRank score={item.Score}></StarRank>
                                    </View>
                                </View>
                                <View style={editable[i] ? styles.Editable : styles.Normal}>
                                    <TextInput editable={editable[i]} onChangeText={(text) => {
                                        item.Title = text
                                    }} style={styles.textTitle}>{item.Title}</TextInput>
                                    <TextInput editable={editable[i]} onChangeText={(text) => {
                                        item.Description = text
                                    }}
                                               style={styles.textDesc}>{item.Description}</TextInput>
                                </View>
                                <View style={{marginTop: 30, flexDirection: "row", display: 'flex'}}>
                                    <View style={{flex: 1}}>
                                        <FoldableButton CommentNumber={item.CommentChildNumber} OnPress={() => {
                                            const temp = foldable.slice(0);
                                            temp[i] = !temp[i];
                                            setFoldable(temp)
                                        }}></FoldableButton>
                                    </View>
                                    {editable[i] ?
                                        <TouchableOpacity disabled={!store.getState().isLoggedIn} onPress={() => {
                                            SaveEditedComment(item,i)
                                        }} style={styles.button}><Text
                                            style={styles.buttonText}>Zapisz</Text></TouchableOpacity>
                                        :
                                        <TouchableOpacity disabled={!store.getState().isLoggedIn} onPress={() => {
                                            setItemForModal(item)
                                            setModalVisible(true)
                                        }}
                                                          style={styles.button}><Text style={styles.buttonText}>Dodaj
                                            komentarz</Text></TouchableOpacity>}
                                </View>
                                <View style={{marginLeft: 30}}>
                                    {foldable[i] ? <CommentForComment
                                        commentUrl={COMMENT + "AllComentsForParent/" + item.Id}></CommentForComment> : null}
                                </View>
                            </View>
                        </View>
                    );
                })}
                <Modal
                    animationType="slide"
                    transparent={true}
                    visible={ModalVisible}
                    onRequestClose={() => {
                        setModalVisible(!ModalVisible);
                    }}>
                    <CommentForCommentFillable touristSpottId={ItemForModal?.TouristSpotId as number}
                                               ParentCommentId={ItemForModal?.Id as number} CloseModal={() => {
                        setModalVisible(false);
                        fetchData()
                    }}></CommentForCommentFillable>
                </Modal>
            </View>
        </View>
    );
});

export default Comment;

const styles = StyleSheet.create({
    main: {
        backgroundColor: COLORS.main,
    },
    container: {},
    column: {
        justifyContent: "center",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        alignContent: "center",
    },
    row: {

        borderWidth: 1,
        borderColor: "#ddd",
        borderRadius: 10,
        justifyContent: "flex-start",
        display: "flex",
        flexDirection: "column",
        alignContent: "center",
        marginTop: 20,
        margin: 10,
        padding: 20,
    },
    commentContent: {
        display: "flex",
        flexDirection: "column",
    },
    row2: {
        width: "100%",
        justifyContent: "space-between",
        display: "flex",
        flexDirection: "row",
        marginTop: 20,
    },
    partRow1: {
        justifyContent: "flex-start",
        display: "flex",
        flexDirection: "row",
        alignItems: "flex-start",
        alignContent: "flex-start",
        marginTop: 20,
    },
    partRow2: {
        justifySelf: "flex-end",
        display: "flex",
        flexDirection: "row",
        alignItems: "flex-end",
        alignContent: "flex-end",
        marginTop: 20,
    },
    counterText: {
        textAlign: "center",
        fontSize: 22,
        color: COLORS.second,
        fontWeight: 800,
        marginLeft: 20,
        marginRight: 20,
    },
    icon: {
        margin: 0,
        fontSize: 30,
        color: COLORS.fourth,


    },
    dishContainer: {
        justifyContent: "center",
        display: "flex",
        flexDirection: "row",
        flexWrap: "wrap",
        alignItems: "center",
        alignContent: "center",
        backgroundColor: "white",
        margin: 0,
    },
    textTitle: {
        fontSize: 24,
        color: COLORS.second,
        marginTop: 20,
    },
    textTitleEdit: {
        fontSize: 24,
        color: COLORS.second,
        marginTop: 20,
        borderColor: COLORS.second,
        borderRadius: 20,
        borderWidth: 2,
        backgroundColor: "white-smoke",
    },
    Editable: {
        borderColor: COLORS.second,
        borderRadius: 20,
        borderWidth: 2,
        backgroundColor: "white-smoke",
    },
    Normal: {},
    textLogin: {
        fontSize: 16,
        color: COLORS.second,
        marginTop: 14,
        marginLeft: 20,
    },
    textDesc: {
        fontSize: 16,
        color: "black",
    },
    textDescEditable: {
        fontSize: 16,
        color: "black",
        borderColor: COLORS.second,
        borderRadius: 20,
        borderWidth: 2,
        backgroundColor: "white-smoke",
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
        textAlign: "center",
        fontSize: 22,
        color: COLORS.second,
    },
    imageContainer: {
        justifyContent: "center",
        display: "flex",
        flexDirection: "column",
        flexWrap: "wrap",
        alignItems: "center",
        alignContent: "center",
        width: "100%",
    },
    elevation: {
        shadowColor: "#ff0000",
        elevation: 20,
    },
    image: {
        width: "100%",
        borderRadius: 5,
        borderColor: COLORS.main,
        borderWidth: 1,
    },
    button: {
        justifyContent: "center",
        flex: 1,
        height: 40,
        borderRadius: 20,
        backgroundColor: COLORS.second,
    },
    buttonText: {
        textAlign: "center",
        color: "white",
        fontSize: 18,
        fontWeight: '800',
    },
    buttonIcon: {
        color: COLORS.second,
        fontSize: 20,
    },
    rowButton: {
        justifyContent: "center",
        display: "flex",
        flexDirection: "row",
    },
});
