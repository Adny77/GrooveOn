// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deezer_album_details.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DeezerAlbumDetails _$DeezerAlbumDetailsFromJson(Map<String, dynamic> json) =>
    DeezerAlbumDetails(
      id: (json['id'] as num).toInt(),
      title: json['title'] as String,
      upc: json['upc'] as String?,
      link: json['link'] as String?,
      share: json['share'] as String?,
      cover: json['cover'] as String?,
      coverSmall: json['cover_small'] as String?,
      coverMedium: json['cover_medium'] as String?,
      coverBig: json['cover_big'] as String?,
      coverXl: json['cover_xl'] as String?,
      genreId: (json['genre_id'] as num?)?.toInt(),
      genres: json['genres'] == null
          ? []
          : DeezerAlbumDetails._genresFromJson(json['genres']),
      label: json['label'] as String?,
      nbTracks: (json['nb_tracks'] as num?)?.toInt(),
      duration: (json['duration'] as num?)?.toInt(),
      fans: (json['fans'] as num?)?.toInt(),
      releaseDate: json['release_date'] as String?,
      recordType: json['record_type'] as String?,
      available: json['available'] as bool?,
      artist: json['artist'] == null
          ? null
          : DeezerArtist.fromJson(json['artist'] as Map<String, dynamic>),
      tracks: DeezerTracksContainer.fromJson(
        json['tracks'] as Map<String, dynamic>,
      ),
    );

Map<String, dynamic> _$DeezerAlbumDetailsToJson(DeezerAlbumDetails instance) =>
    <String, dynamic>{
      'id': instance.id,
      'title': instance.title,
      'upc': instance.upc,
      'link': instance.link,
      'share': instance.share,
      'cover': instance.cover,
      'cover_small': instance.coverSmall,
      'cover_medium': instance.coverMedium,
      'cover_big': instance.coverBig,
      'cover_xl': instance.coverXl,
      'genre_id': instance.genreId,
      'genres': instance.genres,
      'label': instance.label,
      'nb_tracks': instance.nbTracks,
      'duration': instance.duration,
      'fans': instance.fans,
      'release_date': instance.releaseDate,
      'record_type': instance.recordType,
      'available': instance.available,
      'artist': instance.artist,
      'tracks': instance.tracks,
    };

DeezerTracksContainer _$DeezerTracksContainerFromJson(
  Map<String, dynamic> json,
) => DeezerTracksContainer(
  data: (json['data'] as List<dynamic>)
      .map((e) => DeezerTrack.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$DeezerTracksContainerToJson(
  DeezerTracksContainer instance,
) => <String, dynamic>{'data': instance.data};
